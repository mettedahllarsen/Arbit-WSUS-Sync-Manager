using System;
using System.Threading;
using Microsoft.PackageGraph.MicrosoftUpdate.Metadata;
using Microsoft.PackageGraph.MicrosoftUpdate.Source;
using Microsoft.PackageGraph.Storage.Local;
using System.Linq;
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        FetchUpdates();
    }

    static void FetchUpdates()
    {
        // Create a categories source from the Microsoft Update Catalog
        UpstreamCategoriesSource categoriesSource = new(Endpoint.Default);

        // Create a local store to save categories locally
        using var packageStore = PackageStore.OpenOrCreate("./store");

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


        void FindingCategoryAndClassification()
        {
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
    }
}
