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
using Microsoft.PackageGraph.Storage.Azure;
using System.Formats.Tar;
using SharpCompress.Writers;
using SharpCompress.Writers.Tar;
using SharpCompress.Common;
using SharpCompress.Archives.Tar;
using SharpCompress.Archives;
using Newtonsoft.Json.Linq;
using Microsoft.CodeAnalysis;
using System.Text.Json;


string ConnString = "server = localhost; database = WSUSUpdateTable; user id = Frost; password = Frost3310peb; TrustServerCertificate = True";

GetAvailableUpdatesForWindows();
//InsertMetaDataIntoDatabase();
InsertContentMetaDataIntoDatabase();
PrintSupersededUpdates();
DownloadUpdateContent();
//DownloadAllUpdateContent();


void GetAvailableUpdatesForWindows()
{
    // Create a categories source from the Microsoft Update Catalog
    UpstreamCategoriesSource categoriesSource = new(Endpoint.Default);

    // Create a local store to save categories and updates locally
    using var packageStore = Microsoft.PackageGraph.Storage.Local.PackageStore.OpenOrCreate(@"C:\WSUSUpdates");
    categoriesSource.MetadataCopyProgress += PackageStore_MetadataCopyProgress;

    // Copy categories from the upstream source to the local store
    Console.WriteLine("Fetching categories from upstream and saving them to the local store...");
    categoriesSource.CopyTo(packageStore, CancellationToken.None);
    Console.WriteLine();
    Console.WriteLine($"Copied {packageStore.GetPendingPackages().Count} new categories");

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
        Console.WriteLine("Starting copy operation with a timeout of 10 minutes...");
        var startTime = DateTime.Now;

        try
        {
            // Copy updates from the upstream to the local store with the cancellation token
            updatesSource.CopyTo(packageStore, cts.Token);
            Console.WriteLine($"Copied {packageStore.GetPendingPackages().Count} new updates");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine($"Operation timed out after {DateTime.Now - startTime}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

}


//Kan bruges til at lave filtering - Easy. Er dog ikke sikker på at vi skal bruge den endnu (Kommer an på om vi laver filtering på front eller backend.)
ProductCategory FindProduct(string productName, string parentProductName)
{
    using var packageStore = Microsoft.PackageGraph.Storage.Local.PackageStore.OpenOrCreate(@"C:\WSUSUpdates");
    var parentProduct = packageStore
        .OfType<ProductCategory>()
        .FirstOrDefault(category => category.Title.Equals(parentProductName));

    if (parentProduct == null)
    {
        return null;
    }

    return packageStore
        .OfType<ProductCategory>()
        .FirstOrDefault(category => category.Categories.Contains(parentProduct.Id.ID) &&
        category.Title.Equals(productName));
}


void InsertMetaDataIntoDatabase()
{
    // Specify the directory you want to manipulate.
    string path = @"C:\WSUSUpdates\0\metadata\partitions\MicrosoftUpdate";


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

            // Check if the update already exists in the database
            using (SqlCommand checkCommand = new SqlCommand("SELECT COUNT(*) FROM Updates WHERE UpdateID = @UpdateID AND RevisionNumber = @RevisionNumber", connection))
            {
                checkCommand.Parameters.AddWithValue("@UpdateID", updateData.UpdateID != null ? (object)updateData.UpdateID.ID : DBNull.Value);
                checkCommand.Parameters.AddWithValue("@RevisionNumber", updateData.UpdateID != null ? updateData.UpdateID.Revision : (object)DBNull.Value);
                int existingCount = (int)checkCommand.ExecuteScalar();
                bool skipped = false;

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
                            Console.Write("Invalid CreationDate value");
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

                else
                {
                    // An insertion was skipped
                    skipped = true;
                }

                processedFiles++;
                double progressPercentage = (double)processedFiles / totalFiles * 100;
                // Append "- Skipping" to the progress message if an insertion was skipped
                string progressMessage = skipped ? $"Progress: {progressPercentage:F2}% ({processedFiles}/{totalFiles}) - Skipping" : $"Progress: {progressPercentage:F2}% ({processedFiles}/{totalFiles})";
                Console.Write($"\r{progressMessage}");
            }
        }
    }

     

    // Output a success message after all files have been processed
    Console.WriteLine($"Successfully inserted {successfulInserts} metadata updates into the database.");
}


void InsertContentMetaDataIntoDatabase()
{
    // Specify the directory you want to manipulate.
    string path = @"C:\WSUSUpdates\0\filemetadata\partitions\MicrosoftUpdate";

    // Get all .json files in the directory and its subdirectories
    var jsonFiles = Directory.EnumerateFiles(path, "*.json", SearchOption.AllDirectories);
    Console.WriteLine("Trying to insert metadata into database, please wait...");

    int successfulInserts = 0; // Counter for successful inserts
    int totalFiles = jsonFiles.Count(); // Calculate the total number of files
    int processedFiles = 0; // Counter for processed files

    // Establish a connection to your database
    using (SqlConnection connection = new SqlConnection(ConnString))
    {
        connection.Open();

        // Loop through the files
        foreach (string file in jsonFiles)
        {
            // Parse the JSON data
            var fileUpdateData = ParseFileUpdateDataFromJson(file);

            // Check if the exact record already exists in the database
            using (SqlCommand checkCommand = new SqlCommand("SELECT COUNT(*) FROM UpdatesFileMetaData WHERE FileName = @FileName AND Size = @Size AND ModifiedDate = @ModifiedDate AND PatchingType = @PatchingType AND SHA1Hash = @SHA1Hash AND SHA256Hash = @SHA256Hash AND MUUrl = @MUUrl AND USSUrl = @USSUrl", connection))
            {
                checkCommand.Parameters.AddWithValue("@FileName", fileUpdateData.FileName ?? (object)DBNull.Value);
                checkCommand.Parameters.AddWithValue("@Size", fileUpdateData.Size);
                checkCommand.Parameters.AddWithValue("@ModifiedDate", fileUpdateData.ModifiedDate);
                checkCommand.Parameters.AddWithValue("@PatchingType", fileUpdateData.PatchingType ?? (object)DBNull.Value);
                checkCommand.Parameters.AddWithValue("@SHA1Hash", fileUpdateData.SHA1Hash ?? (object)DBNull.Value);
                checkCommand.Parameters.AddWithValue("@SHA256Hash", fileUpdateData.SHA256Hash ?? (object)DBNull.Value);
                checkCommand.Parameters.AddWithValue("@MUUrl", fileUpdateData.MUUrl ?? (object)DBNull.Value);
                checkCommand.Parameters.AddWithValue("@USSUrl", fileUpdateData.USSUrl ?? (object)DBNull.Value);

                int existingCount = (int)checkCommand.ExecuteScalar();
                bool skipped = false;

                // If the count is 0, then the record does not exist in the database
                if (existingCount == 0)
                {
                    // Create a command to insert the update into your database
                    using (SqlCommand command = new SqlCommand("INSERT INTO UpdatesFileMetaData (FileName, Size, ModifiedDate, PatchingType, SHA1Hash, SHA256Hash, MUUrl, USSUrl) VALUES (@FileName, @Size, @ModifiedDate, @PatchingType, @SHA1Hash, @SHA256Hash, @MUUrl, @USSUrl)", connection))
                    {
                        command.Parameters.AddWithValue("@FileName", fileUpdateData.FileName);
                        command.Parameters.AddWithValue("@Size", fileUpdateData.Size);
                        command.Parameters.AddWithValue("@ModifiedDate", fileUpdateData.ModifiedDate);
                        command.Parameters.AddWithValue("@PatchingType", fileUpdateData.PatchingType);
                        command.Parameters.AddWithValue("@SHA1Hash", fileUpdateData.SHA1Hash);
                        command.Parameters.AddWithValue("@SHA256Hash", fileUpdateData.SHA256Hash);
                        command.Parameters.AddWithValue("@MUUrl", fileUpdateData.MUUrl);
                        command.Parameters.AddWithValue("@USSUrl", fileUpdateData.USSUrl);

                        // Execute your SQL command here
                        command.ExecuteNonQuery();
                        successfulInserts++; // Increment the counter for successful inserts
                    }
                }
                else
                {
                    // An insertion was skipped
                    skipped = true;
                }

                processedFiles++;
                double progressPercentage = (double)processedFiles / totalFiles * 100;
                // Append "- Skipping" to the progress message if an insertion was skipped
                string progressMessage = skipped ? $"Progress: {progressPercentage:F2}% ({processedFiles}/{totalFiles}) - Skipping" : $"Progress: {progressPercentage:F2}% ({processedFiles}/{totalFiles})";
                Console.Write($"\r{progressMessage}");
            }
        }
    }

    // Output a success message after all files have been processed
    Console.WriteLine($"Successfully inserted {successfulInserts} file metadata updates into the database.");
}



static void PrintSupersededUpdates()
{
    // Open the local updates store
    using var packageStore = Microsoft.PackageGraph.Storage.Local.PackageStore.Open(@"C:\WSUSUpdates");

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
    using var packageStore = Microsoft.PackageGraph.Storage.Local.PackageStore.Open(@"C:\WSUSUpdates");

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

    // Download the file
    contentStore.Download(new List<IContentFile> { contentFileToDownload }, CancellationToken.None);

    // Get the path of the downloaded file
    string downloadedFilePath = contentStore.GetUri(contentFileToDownload);

    try
    {
        // After download, access the .DONE file and grab the content
        string doneFilePath = Path.Combine(Path.GetDirectoryName(downloadedFilePath), Path.GetFileNameWithoutExtension(downloadedFilePath) + ".DONE");
        if (File.Exists(doneFilePath))
        {
            string content = File.ReadAllText(doneFilePath);

            // Rename the file of type file to the content
            string newFilePath = Path.Combine(Path.GetDirectoryName(downloadedFilePath), content);
            File.Move(downloadedFilePath, newFilePath);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }



    // Retrieve the metadata from the packageStore
    var metadata = packageStore.GetPackage(updateWithContent.Id) as Microsoft.PackageGraph.MicrosoftUpdate.Metadata.SoftwareUpdate;
    Console.WriteLine($"Retrieved metadata for {contentFileToDownload.FileName}: {metadata}");

    // Save the metadata to a JSON file in the same directory as the update file
    var updateFilePath = Path.Combine(@"C:\\UpdateContent", contentFileToDownload.FileName);
    var metadataFilePath = Path.ChangeExtension(updateFilePath, ".json");
    try
    {
        SaveMetadataToJson(metadata, metadataFilePath);
    }
    catch (System.Exception ex)
    {
        // Handle the serialization exception here 
    }

    // This is the original metadata file
    string sourceFile = metadataFilePath;

    string rootDirectory = @"C:\\UpdateContent"; 
    bool doneFileFound = false;

    try
    {
        // This will search the root directory and all its subdirectories
        foreach (string file in Directory.EnumerateFiles(rootDirectory, "*.DONE", SearchOption.AllDirectories))
        {
            Console.WriteLine($"Found .DONE file at: {file}");

            // Read the contents of the .DONE file
            string doneFileContents = File.ReadAllText(file).Trim();
            string[] extensions = { ".exe", ".esd", ".msi", ".csv" };
            if (extensions.Any(extension => doneFileContents.EndsWith(extension)))
            {
                doneFileContents = doneFileContents.Substring(0, doneFileContents.Length - 4);
            }

            // Get the name of the JSON file without the extension
            string jsonFileName = Path.GetFileNameWithoutExtension(sourceFile);

            // Compare the contents of the .DONE file with the name of the JSON file
            if (doneFileContents == jsonFileName)
            {
                Console.WriteLine($"The .DONE file contents match the JSON file name.");
                doneFileFound = true;

                // Copy the JSON file to the directory of the .DONE file
                string destinationFile = Path.Combine(Path.GetDirectoryName(file), Path.GetFileName(sourceFile));
                File.Copy(sourceFile, destinationFile, true);
                Console.WriteLine($"Copied JSON file to: {destinationFile}");

                
                break;
            }
        }

        // If no matching .DONE file was found, throw an error
        if (!doneFileFound)
        {
            throw new Exception("No matching .DONE file was found.");
        }
    }
    catch (Exception e)
    {
        Console.WriteLine($"An error occurred: {e.Message}");
    }

    // Delete the original metadata file
    File.Delete(sourceFile);
    Console.WriteLine($"Deleted original metadata file: {sourceFile}");

    // After downloading, convert all files in C:\UpdateContent to TAR files
    ConvertFilesToTar(@"C:\UpdateContent");
}

static void DownloadAllUpdateContent()
{
    // Open the local updates store
    using var packageStore = Microsoft.PackageGraph.Storage.Local.PackageStore.Open(@"C:\WSUSUpdates");

    // Grab all updates that have some content
    var updatesWithContent = packageStore
        .OfType<Microsoft.PackageGraph.MicrosoftUpdate.Metadata.SoftwareUpdate>()
        .Where(update => update.Files?.Count() > 0);

    if (!updatesWithContent.Any())
    {
        Console.WriteLine("No update in the store has content");
        return;
    }

    var contentStore = new FileSystemContentStore(@"C:\UpdateContent");
    contentStore.Progress += ContentStore_Progress;

    foreach (var updateWithContent in updatesWithContent)
    {
        foreach (var contentFileToDownload in updateWithContent.Files)
        {
            Console.WriteLine($"Downloading {contentFileToDownload.FileName}, size {contentFileToDownload.Size}");

            // Download the file
            contentStore.Download(new List<IContentFile> { contentFileToDownload }, CancellationToken.None);

            // Get the path of the downloaded file
            string downloadedFilePath = contentStore.GetUri(contentFileToDownload);

            try
            {
                // After download, access the .DONE file and grab the content
                string doneFilePath = Path.Combine(Path.GetDirectoryName(downloadedFilePath), Path.GetFileNameWithoutExtension(downloadedFilePath) + ".DONE");
                if (File.Exists(doneFilePath))
                {
                    string content = File.ReadAllText(doneFilePath);

                    // Rename the file of type file to the content
                    string newFilePath = Path.Combine(Path.GetDirectoryName(downloadedFilePath), content);
                    File.Move(downloadedFilePath, newFilePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            // Retrieve the metadata from the packageStore
            var metadata = packageStore.GetPackage(updateWithContent.Id) as Microsoft.PackageGraph.MicrosoftUpdate.Metadata.SoftwareUpdate;
            Console.WriteLine($"Retrieved metadata for {contentFileToDownload.FileName}: {metadata}");

            // Save the metadata to a JSON file in the same directory as the update file
            var updateFilePath = Path.Combine(@"C:\\UpdateContent", contentFileToDownload.FileName);
            var metadataFilePath = Path.ChangeExtension(updateFilePath, ".json");
            try
            {
                SaveMetadataToJson(metadata, metadataFilePath);
            }
            catch (System.Exception ex)
            {
                // Handle the serialization exception here 
            }

            // This is the original metadata file
            string sourceFile = metadataFilePath;

            string rootDirectory = @"C:\\UpdateContent";
            bool doneFileFound = false;

            try
            {
                // This will search the root directory and all its subdirectories
                foreach (string file in Directory.EnumerateFiles(rootDirectory, "*.DONE", SearchOption.AllDirectories))
                {
                    Console.WriteLine($"Found .DONE file at: {file}");

                    // Read the contents of the .DONE file
                    string doneFileContents = File.ReadAllText(file).Trim();
                    string[] extensions = { ".exe", ".esd", ".msi", ".csv"};
                    if (extensions.Any(extension => doneFileContents.EndsWith(extension)))
                    {
                        doneFileContents = doneFileContents.Substring(0, doneFileContents.Length - 4);
                    }

                    // Get the name of the JSON file without the extension
                    string jsonFileName = Path.GetFileNameWithoutExtension(sourceFile);

                    // Compare the contents of the .DONE file with the name of the JSON file
                    if (doneFileContents == jsonFileName)
                    {
                        Console.WriteLine($"The .DONE file contents match the JSON file name.");
                        doneFileFound = true;

                        // Copy the JSON file to the directory of the .DONE file
                        string destinationFile = Path.Combine(Path.GetDirectoryName(file), Path.GetFileName(sourceFile));
                        File.Copy(sourceFile, destinationFile, true);
                        Console.WriteLine($"Copied JSON file to: {destinationFile}");

                        break;
                    }
                }

                // If no matching .DONE file was found, throw an error
                if (!doneFileFound)
                {
                    throw new Exception("No matching .DONE file was found.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e.Message}");
            }

            // Delete the original metadata file
            File.Delete(sourceFile);
            Console.WriteLine($"Deleted original metadata file: {sourceFile}");
        }
    }

    // After downloading, convert all files in C:\UpdateContent to TAR files
    ConvertFilesToTar(@"C:\UpdateContent");
}


static void SaveMetadataToJson(Microsoft.PackageGraph.MicrosoftUpdate.Metadata.SoftwareUpdate metadata, string filePath)
{
    var json = System.Text.Json.JsonSerializer.Serialize(metadata, new JsonSerializerOptions());
    File.WriteAllText(filePath, json);
}

static void ConvertFilesToTar(string startingDirectory)
{
    
    var directories = Directory.GetDirectories(startingDirectory, "*", SearchOption.AllDirectories);

    foreach (var directory in directories)
    {
        // Check if the directory contains any files
        var files = Directory.GetFiles(directory, "*", SearchOption.TopDirectoryOnly);
        if (files.Length > 0)
        {
            // Create a TAR file for the directory
            var tarFileName = Path.Combine(directory, Path.GetFileName(directory) + ".tar");
            using (var archive = TarArchive.Create())
            {
                // Add all files in the directory to the TAR archive
                foreach (var file in files)
                {
                    var entryPath = Path.GetFileName(file);
                    archive.AddEntry(entryPath, file);
                }

                // Save the TAR archive
                archive.SaveTo(tarFileName, new WriterOptions(CompressionType.None));
            }

            Console.WriteLine($"Created TAR file: {tarFileName}");

            // Delete the original files after they have been copied into the TAR directory
            foreach (var file in files)
            {
                try
                {
                    File.Delete(file);
                    Console.WriteLine($"Deleted original file: {file}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to delete original file: {file}. Error: {ex.Message}");
                }
            }
        }

    }
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

   
    var namespaceManager = new XmlNamespaceManager(xmlDoc.NameTable);
    namespaceManager.AddNamespace("upd", "http://schemas.microsoft.com/msus/2002/12/Update");

    var updateIdNode = xmlDoc.SelectSingleNode("//upd:UpdateIdentity/@UpdateID", namespaceManager);
     var revisionNumberNode = xmlDoc.SelectSingleNode("//upd:UpdateIdentity/@RevisionNumber", namespaceManager); 

    if (updateIdNode != null && revisionNumberNode != null) 
    {
        var id = Guid.Parse(updateIdNode.Value);
        int revisionNumber = int.Parse(revisionNumberNode.Value); 
        updateData.UpdateID = new MicrosoftUpdatePackageIdentity(id, revisionNumber); 
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


static FileUpdateData ParseFileUpdateDataFromJson(string filePath)
{
    var fileUpdateData = new FileUpdateData();

    // Load the JSON file
    var jsonContent = File.ReadAllText(filePath);

    var jsonArray = JArray.Parse(jsonContent);

    // Assuming you want to process the first object in the array
    var jsonObject = jsonArray[0];

    // Check if the properties exist and are not null before assigning them
    fileUpdateData.FileName = jsonObject["FileName"]?.ToString() ?? string.Empty;
    fileUpdateData.Size = jsonObject["Size"]?.ToObject<long>() ?? 0;
    fileUpdateData.ModifiedDate = jsonObject["ModifiedDate"]?.ToObject<DateTime>() ?? DateTime.MinValue;
    fileUpdateData.PatchingType = jsonObject["PatchingType"]?.ToString() ?? string.Empty;

    var digests = jsonObject["Digests"] as JArray;
    if (digests != null && digests.Count > 0)
    {
        var sha1Digest = digests.FirstOrDefault(d => d["Algorithm"].ToString() == "SHA1");
        if (sha1Digest != null)
        {
            fileUpdateData.SHA1Hash = sha1Digest["HexString"]?.ToString() ?? string.Empty;
        }

        var sha256Digest = digests.FirstOrDefault(d => d["Algorithm"].ToString() == "SHA256");
        if (sha256Digest != null)
        {
            fileUpdateData.SHA256Hash = sha256Digest["HexString"]?.ToString() ?? string.Empty;
        }
    }

    var urls = jsonObject["Urls"] as JArray;
    if (urls != null && urls.Count > 0)
    {
        var muUrlObject = urls.FirstOrDefault();
        if (muUrlObject != null)
        {
            fileUpdateData.MUUrl = muUrlObject["MuUrl"]?.ToString() ?? string.Empty;
            fileUpdateData.USSUrl = muUrlObject["UssUrl"]?.ToString() ?? string.Empty;
        }
    }

    return fileUpdateData;
}



