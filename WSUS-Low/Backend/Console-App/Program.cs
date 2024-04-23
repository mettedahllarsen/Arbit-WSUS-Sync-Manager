using Microsoft.PackageGraph.MicrosoftUpdate.Metadata;
using Microsoft.PackageGraph.MicrosoftUpdate.Source;
using Microsoft.PackageGraph.ObjectModel;
using Microsoft.PackageGraph.Storage.Local;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Xml;
using System.IO.Compression;
using Console_App;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.PackageGraph.MicrosoftUpdate.Endpoints.Content;
using Microsoft.Extensions.Logging;
using Microsoft.PackageGraph.Storage;
using System.Text;
using System.Threading.Tasks;
using Microsoft.PackageGraph.MicrosoftUpdate.Metadata;
using Microsoft.PackageGraph.MicrosoftUpdate.Source;
using Microsoft.PackageGraph.ObjectModel;
using Microsoft.PackageGraph.Storage.Local;
using Microsoft.UpdateServices.Metadata;
using Microsoft.UpdateServices.WebServices.ClientSync;


string ConnString = "server = localhost; database = WSUSUpdateTable; user id = Frost; password = Frost3310peb; TrustServerCertificate = True";

GetAvailableUpdatesForWindows();
InsertIntoDatabase();
PrintSupersededUpdates();
DownloadUpdateContent();


void GetAvailableUpdatesForWindows()
{
    // Create a categories source from the Microsoft Update Catalog
    UpstreamCategoriesSource categoriesSource = new(Endpoint.Default);

    // Create a local store to save categories and updates locally
    using var packageStore = PackageStore.OpenOrCreate(@"C:\WSUSUpdates");
    categoriesSource.MetadataCopyProgress += PackageStore_MetadataCopyProgress;

    // Copy categories from the upstream source to the local store
    Console.WriteLine("Fetching categories from upstream and saving them to the local store...");
    categoriesSource.CopyTo(packageStore, CancellationToken.None);
    Console.WriteLine();
    Console.WriteLine($"Copied {packageStore.GetPendingPackages().Count} new categories");

    // Flush not required; done here for demonstration purposes to clear the pending package count
    packageStore.Flush();

    // Create a filter to retrieve selected updates by product name
    var updatesFilter = new UpstreamSourceFilter();

    // Set a "windows 11" product filter.
    // First find the "Windows" product
    var windowsProduct = packageStore
        .OfType<ProductCategory>()
        .First(category => category.Title.Equals("Windows"));
    // Find the "Windows 11" product that is a child of "Windows"
    var windows11Product = packageStore
        .OfType<ProductCategory>()
        .First(category => category.Categories.Contains(windowsProduct.Id.ID) &&
        category.Title.Equals("Windows 11"));
    updatesFilter.ProductsFilter.Add(windows11Product.Id.ID);

    // Allow all available update classifications for the product selected
    updatesFilter
        .ClassificationsFilter
        .AddRange(packageStore.OfType<ClassificationCategory>().Select(classification => classification.Id.ID));
    Console.WriteLine($"Filtering to product \"{windows11Product.Title}\", all  classifications.");

    // Create an upstream updates source from the Microsoft Update Catalog
    Console.WriteLine("Fetching matching updates from upstream and saving them to the local store...");
    UpstreamUpdatesSource updatesSource = new(Endpoint.Default, updatesFilter);
    updatesSource.MetadataCopyProgress += PackageStore_MetadataCopyProgress;


    var timeout = TimeSpan.FromMinutes(10);

    // Create a CancellationTokenSource with a timeout of 10 minutes
    using (var cts = new CancellationTokenSource(timeout))
    {
        // Copy updates from the upstream to the local store with the cancellation token
        updatesSource.CopyTo(packageStore, cts.Token);
        Console.WriteLine();
        Console.WriteLine($"Copied {packageStore.GetPendingPackages().Count} new updates");
    }
}

void InsertIntoDatabase()
{
    // Specify the directory you want to manipulate.
    string path = @"C:\WSUSUpdates\metadata\partitions\MicrosoftUpdate";


    // Get all .xml files in the directory and its subdirectories
    var xmlFiles = Directory.EnumerateFiles(path, "*.xml", SearchOption.AllDirectories);
    Console.WriteLine("Trying to insert metadata into database, please wait...");

    int successfulInserts = 0; // Counter for successful inserts
    int totalFiles = xmlFiles.Count(); // Calculate the total number of files
    int processedFiles = 0; // Counter for processed files

    // Establish a connection to your database
    using (SqlConnection connection = new SqlConnection(ConnString))
    {
        connection.Open();

        // Loop through the files
        foreach (string file in xmlFiles)
        {
            // Read the file
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(file);

            // Parse the JSON data
            var updateData = ParseUpdateDataFromXml(xmlDoc);

            // Create a command to check if the update already exists in your database
            using (SqlCommand checkCommand = new SqlCommand("SELECT COUNT(*) FROM Updates WHERE UpdateID = @UpdateID AND RevisionNumber = @RevisionNumber", connection))
            {
                checkCommand.Parameters.AddWithValue("@UpdateID", updateData.UpdateID != null ? (object)updateData.UpdateID.ID : DBNull.Value);
                checkCommand.Parameters.AddWithValue("@RevisionNumber", updateData.RevisionNumber != 0 ? updateData.RevisionNumber : (object)DBNull.Value);
                // Execute the command and get the count
                int existingCount = (int)checkCommand.ExecuteScalar();
                // If the count is 0, then the update does not exist in the database
                if (existingCount == 0)
                {
                    // Create a command to insert the update into your database
                    using (SqlCommand command = new SqlCommand("INSERT INTO Updates (UpdateID, RevisionNumber, DefaultPropertiesLanguage, UpdateType, MaxDownloadSize, MinDownloadSize, PublicationState, CreationDate, PublisherID, Title) VALUES (@UpdateID, @RevisionNumber, @DefaultPropertiesLanguage, @UpdateType, @MaxDownloadSize, @MinDownloadSize, @PublicationState, @CreationDate, @PublisherID, @Title)", connection))
                    {
                        command.Parameters.AddWithValue("@UpdateID", updateData.UpdateID != null ? (object)updateData.UpdateID.ID : DBNull.Value);
                        command.Parameters.AddWithValue("@RevisionNumber", updateData.UpdateID != null ? updateData.UpdateID.Revision : (object)DBNull.Value);
                        command.Parameters.AddWithValue("@DefaultPropertiesLanguage", !string.IsNullOrEmpty(updateData.DefaultPropertiesLanguage) ? updateData.DefaultPropertiesLanguage : (object)DBNull.Value);
                        command.Parameters.AddWithValue("@UpdateType", !string.IsNullOrEmpty(updateData.UpdateType) ? updateData.UpdateType : (object)DBNull.Value);
                        command.Parameters.AddWithValue("@MaxDownloadSize", updateData.MaxDownloadSize != 0 ? updateData.MaxDownloadSize : (object)DBNull.Value);
                        command.Parameters.AddWithValue("@MinDownloadSize", updateData.MinDownloadSize != 0 ? updateData.MinDownloadSize : (object)DBNull.Value);
                        command.Parameters.AddWithValue("@PublicationState", !string.IsNullOrEmpty(updateData.PublicationState) ? updateData.PublicationState : (object)DBNull.Value);

                        // Check if the CreationDate is within the valid range for SQL Server's datetime type
                        if (updateData.CreationDate < new DateTime(1753, 1, 1) || updateData.CreationDate > new DateTime(9999, 12, 31))
                        {
                            Console.WriteLine("Invalid CreationDate value");
                            command.Parameters.AddWithValue("@CreationDate", DBNull.Value); // Set a default value
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@CreationDate", updateData.CreationDate);
                        }

                        command.Parameters.AddWithValue("@PublisherID", updateData.PublisherID != null ? updateData.PublisherID : DBNull.Value);
                        command.Parameters.AddWithValue("@Title", !string.IsNullOrEmpty(updateData.Title) ? updateData.Title : (object)DBNull.Value);

                        // Execute your SQL command here
                        command.ExecuteNonQuery();
                        successfulInserts++; // Increment the counter for successful inserts
                    }
                }
            }
            processedFiles++; // Increment the counter for processed files
            double progressPercentage = (double)processedFiles / totalFiles * 100; // Calculate the progress percentage
            Console.Write($"\rProgress: {progressPercentage:F2}% ({processedFiles}/{totalFiles})"); // Use \r to overwrite the line
        }
    }

     

    // Output a success message after all files have been processed
    Console.WriteLine($"Successfully inserted {successfulInserts} updates into the database.");
}





static void PrintSupersededUpdates()
{
    // Open the local updates store
    using var packageStore = PackageStore.Open(@"C:\WSUSUpdates");

    // Grab the first cumulative update that is superseded by another update
    var firstUpdateAvailable = packageStore
        .OfType<Microsoft.PackageGraph.MicrosoftUpdate.Metadata.SoftwareUpdate>()
        .FirstOrDefault(update => update.IsSupersededBy?.Count > 0 &&
        update.Title.Contains("cumulative", StringComparison.OrdinalIgnoreCase));

    if (firstUpdateAvailable is null)
    {
        Console.WriteLine("No update in the store has been superseded");
        return;
    }

    Console.WriteLine($"Software update in the store: {firstUpdateAvailable.Title}");
    Console.WriteLine($"Superseded by:");
    foreach (var supersededUpdateId in firstUpdateAvailable.IsSupersededBy)
    {
        var supersededByUpdate = packageStore
            .FirstOrDefault(update => update.Id == supersededUpdateId);
        if (supersededByUpdate is not null)
        {
            Console.WriteLine($"    {supersededByUpdate.Title}");
        }
    }
}

static void DownloadUpdateContent()
{
    // Open the local updates store
    using var packageStore = PackageStore.Open(@"C:\WSUSUpdates");

    // Grab the first update that has some content
    var updateWithContent = packageStore
        .OfType<Microsoft.PackageGraph.MicrosoftUpdate.Metadata.SoftwareUpdate>()
        .FirstOrDefault(update => update.Files?.Count() > 0);

    if (updateWithContent is null)
    {
        Console.WriteLine("No update in the store has content");
        return;
    }

    var contentFileToDownload = updateWithContent.Files.First();
    Console.WriteLine($"Downloading {contentFileToDownload.FileName}, size {contentFileToDownload.Size}");

    var contentStore = new FileSystemContentStore(@"C:\UpdateContent");
    contentStore.Progress += ContentStore_Progress;

    contentStore.Download(new List<IContentFile> { contentFileToDownload }, CancellationToken.None);
}

static void ContentStore_Progress(object? sender, Microsoft.PackageGraph.ObjectModel.ContentOperationProgress e)
{
    Console.CursorLeft = 0;
    if (e.CurrentOperation == PackagesOperationType.DownloadFileProgress)
    {
        Console.Write($"Downloading update content {e.Current}/{e.Maximum}");
    }
    else if (e.CurrentOperation == PackagesOperationType.HashFileProgress)
    {
        Console.Write($"Hashing update content {e.Current}/{e.Maximum}");
    }
    else if (e.CurrentOperation == PackagesOperationType.DownloadFileEnd || e.CurrentOperation == PackagesOperationType.HashFileEnd)
    {
        Console.WriteLine();
    }
}

static void PackageStore_MetadataCopyProgress(object? sender, Microsoft.PackageGraph.Storage.PackageStoreEventArgs e)
{
    Console.CursorLeft = 0;
    Console.Write($"Copying package metadata {e.Current}/{e.Total}");
}


static Console_App.UpdateData ParseUpdateDataFromXml(XmlDocument xmlDoc)
{
    var updateData = new Console_App.UpdateData();

    // Create an XmlNamespaceManager to resolve the namespaces in the XML
    var namespaceManager = new XmlNamespaceManager(xmlDoc.NameTable);
    namespaceManager.AddNamespace("upd", "http://schemas.microsoft.com/msus/2002/12/Update");

    var updateIdNode = xmlDoc.SelectSingleNode("//upd:UpdateIdentity/@UpdateID", namespaceManager);
     var revisionNumberNode = xmlDoc.SelectSingleNode("//upd:UpdateIdentity/@RevisionNumber", namespaceManager); // Fetch the RevisionNumber

    if (updateIdNode != null && revisionNumberNode != null) // Check if both nodes are not null
    {
        var id = Guid.Parse(updateIdNode.Value);
        int revisionNumber = int.Parse(revisionNumberNode.Value); // Parse the RevisionNumber to int
        updateData.UpdateID = new MicrosoftUpdatePackageIdentity(id, revisionNumber); // Use the fetched RevisionNumber
    }


    var defaultPropertiesLanguageNode = xmlDoc.SelectSingleNode("//upd:Properties/@DefaultPropertiesLanguage", namespaceManager);
    if (defaultPropertiesLanguageNode != null)
    {
        updateData.DefaultPropertiesLanguage = defaultPropertiesLanguageNode.Value;
    }

    var updateTypeNode = xmlDoc.SelectSingleNode("//upd:Properties/@UpdateType", namespaceManager);
    if (updateTypeNode != null)
    {
        updateData.UpdateType = updateTypeNode.Value;
    }

    var maxDownloadSizeNode = xmlDoc.SelectSingleNode("//upd:Properties/@MaxDownloadSize", namespaceManager);
    if (maxDownloadSizeNode != null)
    {
        updateData.MaxDownloadSize = long.Parse(maxDownloadSizeNode.Value); // Use long.Parse instead of int.Parse
    }

    var minDownloadSizeNode = xmlDoc.SelectSingleNode("//upd:Properties/@MinDownloadSize", namespaceManager);
    if (minDownloadSizeNode != null)
    {
        updateData.MinDownloadSize = long.Parse(minDownloadSizeNode.Value); // Use long.Parse instead of int.Parse
    }

    var publicationStateNode = xmlDoc.SelectSingleNode("//upd:Properties/@PublicationState", namespaceManager);
    if (publicationStateNode != null)
    {
        updateData.PublicationState = publicationStateNode.Value;
    }

    var creationDateNode = xmlDoc.SelectSingleNode("//upd:Properties/@CreationDate", namespaceManager);
    if (creationDateNode != null)
    {
        updateData.CreationDate = DateTime.Parse(creationDateNode.Value, null, System.Globalization.DateTimeStyles.RoundtripKind);
    }

    var publisherIdNode = xmlDoc.SelectSingleNode("//upd:Properties/@PublisherID", namespaceManager);
    if (publisherIdNode != null)
    {
        updateData.PublisherID = Guid.Parse(publisherIdNode.Value);
    }

    var titleNode = xmlDoc.SelectSingleNode("//upd:LocalizedProperties/upd:Title", namespaceManager);
    if (titleNode != null)
    {
        updateData.Title = titleNode.InnerText;
    }

    return updateData;
}



