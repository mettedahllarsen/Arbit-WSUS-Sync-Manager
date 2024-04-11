using Microsoft.PackageGraph.MicrosoftUpdate.Metadata;
using Microsoft.PackageGraph.MicrosoftUpdate.Source;
using Microsoft.PackageGraph.ObjectModel;
using Microsoft.PackageGraph.Storage.Local;
using System.Threading.Tasks;


FetchUpdates();
FindingCategoryAndClassification();
await FetchWindows11Updates();
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


static async Task FetchWindows11Updates()
{
    using var packageStore = PackageStore.OpenOrCreate(@"C:\WSUSUpdates");

    var updatesFilter = new UpstreamSourceFilter();

    var windowsProduct = packageStore.OfType<ProductCategory>().First(category => category.Title.Equals("Windows"));

    var windows11Product = packageStore.OfType<ProductCategory>().First(category => category.Title.Equals("Windows 11"));

    updatesFilter.ProductsFilter.Add(windows11Product.Id.ID);

    updatesFilter.ClassificationsFilter.AddRange(packageStore.OfType<ClassificationCategory>().Select(classification => classification.Id.ID));

    var updatesSource = new UpstreamUpdatesSource(Endpoint.Default, updatesFilter);

    var timeout = TimeSpan.FromMinutes(10);

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
        .FirstOrDefault(update => update.Files?.Count() > 0);

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





