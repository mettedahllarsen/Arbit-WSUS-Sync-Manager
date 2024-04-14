using Microsoft.PackageGraph.MicrosoftUpdate.Metadata;
using Microsoft.PackageGraph.MicrosoftUpdate.Source;
using Microsoft.PackageGraph.ObjectModel;
using Microsoft.PackageGraph.Storage.Local;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Xml;
using System.IO.Compression;

string ConnString = "server = localhost; database = WSUSUpdateTable; user id = Frost; password = Frost3310peb; TrustServerCertificate = True";

FetchUpdates();
FindingCategoryAndClassification();
await FetchWindowsUpdates();
AvailableUpdates();
UpdateWithContent();


static void FetchUpdates()
{
    // Create a categories source from the Microsoft Update Catalog
    UpstreamCategoriesSource categoriesSource = new(Endpoint.Default);

    // Create a local store to save categories locally
    using var packageStore = PackageStore.OpenOrCreate(@"C:\WSUSUpdates");

    Console.WriteLine("Fetching updates...");

    // Copy categories from the upstream source to the local store
    categoriesSource.CopyTo(packageStore, CancellationToken.None);

    var pendingPackages = packageStore.GetPendingPackages();

    int skippedUpdates = 0;

    foreach (var package in pendingPackages)
    {
        try
        {
            // Try to fetch the update
            Console.WriteLine($"Fetched update {package.Title}");
        }
        catch (Exception)
        {
            // If fetching the update fails, increment the skipped updates counter
            skippedUpdates++;
        }
    }



    Console.WriteLine($"Finished fetching all the updates. Total updates fetched: {pendingPackages.Count - skippedUpdates}");
    Console.WriteLine($"Total updates skipped: {skippedUpdates}");

}

async Task FetchWindowsUpdates()
{
    using var packageStore = PackageStore.OpenOrCreate(@"C:\WSUSUpdates");

    var updatesFilter = new UpstreamSourceFilter();

    var windowsProduct = packageStore.OfType<ProductCategory>().First(category => category.Title.Equals("Windows"));

    updatesFilter.ProductsFilter.Add(windowsProduct.Id.ID);

    updatesFilter.ClassificationsFilter.AddRange(packageStore.OfType<ClassificationCategory>().Select(classification => classification.Id.ID));

    var updatesSource = new UpstreamUpdatesSource(Endpoint.Default, updatesFilter);

    var timeout = TimeSpan.FromMinutes(30);

    // Create a cancellation token source with timeout
    using var cancellationTokenSource = new CancellationTokenSource(timeout);

    var copyTask = Task.Run(() => updatesSource.CopyTo(packageStore, cancellationTokenSource.Token));

    // Wait for either the copy operation to complete or the timeout
    await Task.WhenAny(copyTask, Task.Delay(timeout, cancellationTokenSource.Token));

    // If the copy operation is still running, cancel it
    if (!copyTask.IsCompleted)
    {
        cancellationTokenSource.Cancel();
        Console.WriteLine("Copy operation timed out after 10 minutes.");
    }
    else
    {
        Console.WriteLine($"Copied {packageStore.GetPendingPackages().Count} new updates");
    }


    string zipPath = @"C:\WSUSUpdates\0.zip";
    string extractPath = @"C:\WSUSUpdates";
    string extractedFolderPath = @"C:\WSUSUpdates\0";

    // Check if the extracted folder already exists
    if (!Directory.Exists(extractedFolderPath))
    {
        // If the extracted folder doesn't exist, extract the zip file
        ZipFile.ExtractToDirectory(zipPath, extractPath);
    }

    // Specify the directory you want to manipulate.
    string path = @"C:\WSUSUpdates\0\metadata\partitions\MicrosoftUpdate";

    // Get all .xml files in the directory and its subdirectories
    var xmlFiles = Directory.EnumerateFiles(path, "*.xml", SearchOption.AllDirectories);


    // Establish a connection to your database
    using (SqlConnection connection = new SqlConnection(ConnString))
    {
        connection.Open();

        // Loop through the files
        foreach (string file in xmlFiles)
        {
            Console.WriteLine($"Reading {file}");

            // Read the file
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(file);

            // Parse the JSON data
            var updateData = ParseUpdateDataFromXml(xmlDoc);

            // Try to fetch the update
            Console.WriteLine($"Fetched update {updateData.Title}");

            // Create a command to insert the update into your database
            using (SqlCommand command = new SqlCommand("INSERT INTO Updates (UpdateID, RevisionNumber, DefaultPropertiesLanguage, UpdateType, MaxDownloadSize, MinDownloadSize, PublicationState, CreationDate, PublisherID, Title) VALUES (@UpdateID, @RevisionNumber, @DefaultPropertiesLanguage, @UpdateType, @MaxDownloadSize, @MinDownloadSize, @PublicationState, @CreationDate, @PublisherID, @Title)", connection))
            {
                command.Parameters.AddWithValue("@UpdateID", updateData.UpdateID != null ? updateData.UpdateID : DBNull.Value);
                command.Parameters.AddWithValue("@RevisionNumber", updateData.RevisionNumber != 0 ? updateData.RevisionNumber : (object)DBNull.Value);
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
            }

        }
    }
}

static Console_App.UpdateData ParseUpdateDataFromXml(XmlDocument xmlDoc)
{
    var updateData = new Console_App.UpdateData();

    // Create an XmlNamespaceManager to resolve the namespaces in the XML
    var namespaceManager = new XmlNamespaceManager(xmlDoc.NameTable);
    namespaceManager.AddNamespace("upd", "http://schemas.microsoft.com/msus/2002/12/Update");

    var updateIdNode = xmlDoc.SelectSingleNode("//upd:UpdateIdentity/@UpdateID", namespaceManager);
    if (updateIdNode != null)
    {
        updateData.UpdateID = Guid.Parse(updateIdNode.Value);
    }

    var revisionNumberNode = xmlDoc.SelectSingleNode("//upd:UpdateIdentity/@RevisionNumber", namespaceManager);
    if (revisionNumberNode != null)
    {
        updateData.RevisionNumber = int.Parse(revisionNumberNode.Value);
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
        updateData.MaxDownloadSize = long.Parse(maxDownloadSizeNode.Value);
    }

    var minDownloadSizeNode = xmlDoc.SelectSingleNode("//upd:Properties/@MinDownloadSize", namespaceManager);
    if (minDownloadSizeNode != null)
    {
        updateData.MinDownloadSize = long.Parse(minDownloadSizeNode.Value);
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


void FindingCategoryAndClassification()
{
    using var packageStore = PackageStore.OpenOrCreate(@"C:\WSUSUpdates");
    // Find the "Windows" product category.
    var windowsProduct = packageStore
                .OfType<ProductCategory>()
                .First(category => category.Title.Equals("Windows"));

    //Find classifications by name.
    var securityUpdateClassification = packageStore
        .OfType<ClassificationCategory>()
        .Where(classification => classification.Title.Equals("Security Updates"))
        .FirstOrDefault();

}

static void AvailableUpdates()
{

    // Open the local updates store
    using var packageStore = PackageStore.Open(@"C:\WSUSUpdates");

    // Grab the first cumulative update that is superseded by another update
    var firstUpdateAvailable = packageStore
        .OfType<SoftwareUpdate>()
        .FirstOrDefault(update => update.IsSupersededBy?.Count > 0 &&
        update.Title.Contains("cumulative", StringComparison.OrdinalIgnoreCase));

    if (firstUpdateAvailable is not null)
    {
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
}

static void UpdateWithContent()
{
    // Open the local updates store
    // Make sure some updates have been fetched prior
    using var packageStore = PackageStore.Open(@"C:\WSUSUpdates");

    // Grab the first update that has some content
    var updateWithContent = packageStore
        .OfType<SoftwareUpdate>()
        .LastOrDefault(update => update.Files?.Count() > 0);

    if (updateWithContent is null)
    {
        Console.WriteLine("No update in the store has content");
        return;
    }

    var contentFileToDownload = updateWithContent.Files.First();
    Console.WriteLine(
        $"Downloading {contentFileToDownload.FileName}, size {contentFileToDownload.Size}");

    var contentStore = new FileSystemContentStore(@"C:\UpdateContent");
    contentStore.Download(
        new List<IContentFile> { contentFileToDownload },
        CancellationToken.None);
}





