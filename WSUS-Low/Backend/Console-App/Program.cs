using Microsoft.PackageGraph.MicrosoftUpdate.Metadata;
using Microsoft.PackageGraph.MicrosoftUpdate.Source;
using Microsoft.PackageGraph.ObjectModel;
using Microsoft.PackageGraph.Storage.Local;
using SharpCompress.Writers;
using SharpCompress.Common;
using SharpCompress.Archives.Tar;
using SharpCompress.Archives;
using Microsoft.CodeAnalysis;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using JsonSerializer = System.Text.Json.JsonSerializer;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole(); 
});

ILogger logger = loggerFactory.CreateLogger<Program>(); 


//GetAvailableUpdatesForWindows();
//PrintSupersededUpdates();
DownloadUpdateContent();


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
    var updateFilePath = Path.Combine(@"C:\UpdateContent", contentFileToDownload.FileName);
    var metadataFilePath = Path.ChangeExtension(updateFilePath, ".json");

    try
    {
        SaveMetadataToJson(metadata, metadataFilePath);
    }
    catch (System.Exception ex)
    {
        Console.WriteLine($"Failed to save metadata to JSON: {ex.Message}");

        // If saving to JSON fails, try saving to XML
        metadataFilePath = Path.ChangeExtension(updateFilePath, ".xml");
        try
        {
            SaveMetadataToXml(metadata, metadataFilePath);
            Console.WriteLine($"Successfully saved metadata to XML: {metadataFilePath}");
        }
        catch (System.Exception xmlEx)
        {
            Console.WriteLine($"Failed to save metadata to XML: {xmlEx.Message}");
        }
    }

    // This is the original metadata file
    string sourceFile = metadataFilePath;

    string rootDirectory = @"C:\UpdateContent";
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

            // Get the name of the JSON or XML file without the extension
            string metadataFileName = Path.GetFileNameWithoutExtension(sourceFile);

            // Compare the contents of the .DONE file with the name of the metadata file
            if (doneFileContents == metadataFileName)
            {
                Console.WriteLine($"The .DONE file contents match the metadata file name.");
                doneFileFound = true;

                // Copy the metadata file to the directory of the .DONE file
                string destinationFile = Path.Combine(Path.GetDirectoryName(file), Path.GetFileName(sourceFile));
                File.Copy(sourceFile, destinationFile, true);
                Console.WriteLine($"Copied metadata file to: {destinationFile}");

                break;
            }
        }

        // If no matching .DONE file was found, throw an error
        if (!doneFileFound)
        {
            throw new FileNotFoundException("No matching .DONE file was found.");
        }
    }
    catch (FileNotFoundException ex)
    {
        Console.WriteLine($"No matching .DONE file was found: {ex.Message}");
    }
    catch (IOException ex)
    {
        Console.WriteLine($"IO error occurred: {ex.Message}");
    }
    catch (UnauthorizedAccessException ex)
    {
        Console.WriteLine($"Access denied: {ex.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An unexpected error occurred: {ex.Message}");
    }

    // Delete the original metadata file
    File.Delete(sourceFile);
    Console.WriteLine($"Deleted original metadata file: {sourceFile}");

    // After downloading, convert all files in C:\UpdateContent to TAR files
    ConvertFilesToTar(@"C:\UpdateContent");
}


//Standard serializer - Fejler på ukendte attributter
static void SaveMetadataToJsonStandard(SoftwareUpdate metadata, string filePath)
{
    var options = new JsonSerializerOptions
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
    };

    try
    {
        string jsonString = JsonSerializer.Serialize(metadata, options);
        File.WriteAllText(filePath, jsonString);
        Console.WriteLine($"Successfully saved metadata to JSON: {filePath}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to save metadata to JSON: {ex.Message}");
        throw; // Optionally, handle or rethrow the exception as needed
    }
}


//Custom serializer - Bruger CustomSoftwareUpdate...
static void SaveMetadataToJson(SoftwareUpdate metadata, string filePath)
{
    var options = new JsonSerializerOptions
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
    };

    // Customize the converter to handle unknown expression types
    options.Converters.Add(new CustomSoftwareUpdateConverter());

    string jsonString = JsonSerializer.Serialize(metadata, options);
    File.WriteAllText(filePath, jsonString);
}

static void SaveMetadataToXml(SoftwareUpdate metadata, string filePath)
{
    var serializer = new DataContractSerializer(typeof(SoftwareUpdate));
    using (var writer = new FileStream(filePath, FileMode.Create))
    {
        serializer.WriteObject(writer, metadata);
    }
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




