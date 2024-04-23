using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Microsoft.PackageGraph.MicrosoftUpdate.Metadata;
using Microsoft.PackageGraph.MicrosoftUpdate.Source;
using Microsoft.PackageGraph.Storage;
using Microsoft.PackageGraph.Storage.Local;
using System.Xml;
using WSUSLowAPI.Contexts;
using WSUSLowAPI.Models;
using Endpoint = Microsoft.PackageGraph.MicrosoftUpdate.Source.Endpoint;

namespace WSUSLowAPI.Repositories
{
    public class UpdateMetadataRepositoryDb(WSUSDbContext dbcontext) : IUpdateMetadataRepository
    {
        public List<UpdateMetadata> GetAll()
        {
            return [.. dbcontext.UpdateMetadata];
        }

        public string FetchToDb(string filter)
        {
            GetAvailableUpdates(filter);
            return "Successfully";
        }

        private void GetAvailableUpdates(string filter)
        {
            UpstreamCategoriesSource categoriesSource = new(Endpoint.Default);

            using var packageStore = PackageStore.OpenOrCreate(@"C:\WSUSUpdates");
            categoriesSource.MetadataCopyProgress += PackageStore_MetadataCopyProgress;

            Console.WriteLine("Fetching categories from upstream and saving them to the local store...");
            categoriesSource.CopyTo(packageStore, CancellationToken.None);
            Console.WriteLine();
            Console.WriteLine($"Copied {packageStore.GetPendingPackages().Count} new categories");

            packageStore.Flush();

            var updatesFilter = new UpstreamSourceFilter();

            if (!filter.IsNullOrEmpty())
            {
                var filteredProducts = packageStore.OfType<ProductCategory>()
                    .First(category => category.Categories.Equals(filter));
                updatesFilter.ProductsFilter.Add(filteredProducts.Id.ID);

                updatesFilter.ClassificationsFilter.AddRange(
                    packageStore.OfType<ClassificationCategory>()
                    .Select(classification => classification.Id.ID));
                Console.WriteLine($"Filtering to product \"{filteredProducts.Title}\", all  classifications.");

                Console.WriteLine("Fetching matching updates from upstream and saving them to the local store...");
                UpstreamUpdatesSource updatesSource = new(Endpoint.Default, updatesFilter);
                updatesSource.MetadataCopyProgress += PackageStore_MetadataCopyProgress;


                var timeout = TimeSpan.FromMinutes(10);

                using var cts = new CancellationTokenSource(timeout);
                updatesSource.CopyTo(packageStore, cts.Token);
                Console.WriteLine();
                Console.WriteLine($"Copied {packageStore.GetPendingPackages().Count} new updates");
            } else
            {
                Console.WriteLine("Fetching all updates from upstream and saving them to the local store...");
                UpstreamUpdatesSource updatesSource = new(Endpoint.Default, updatesFilter);
                updatesSource.MetadataCopyProgress += PackageStore_MetadataCopyProgress;


                var timeout = TimeSpan.FromMinutes(20);

                using var cts = new CancellationTokenSource(timeout);
                updatesSource.CopyTo(packageStore, cts.Token);
                Console.WriteLine();
                Console.WriteLine($"Copied {packageStore.GetPendingPackages().Count} new updates");
            }
        }

        private static void PackageStore_MetadataCopyProgress(object? sender, PackageStoreEventArgs e)
        {
            Console.CursorLeft = 0;
            Console.WriteLine($"Copying package metadata {e.Current}/{e.Total}");
        }

        private void InsertIntoDatabase()
        {
            string path = @"C:\WSUSUpdates\metadata\partitions\MicrosoftUpdate";

            var xmlFiles = Directory.EnumerateFiles(path, "*.xml", SearchOption.AllDirectories);
            Console.WriteLine("Trying to insert metadata into database, please wait...");

            int successfulInserts = 0;
            int totalFiles = xmlFiles.Count();
            int processedFiles = 0;

            foreach (string  xmlFile in xmlFiles)
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlFile);

                var updateMetadata
            }
        }

        private static UpdateMetadata ParseMetadataFromXml(XmlDocument xmlDoc)
        {
            var updateMetadata = new UpdateMetadata();

            var namespaceManager = new XmlNamespaceManager(xmlDoc.NameTable);
            namespaceManager.AddNamespace("upd", "http://schemas.microsoft.com/msus/2002/12/Update");

            var updateIdNode = xmlDoc.SelectSingleNode("//upd:UpdateIdentity/@UpdateID", namespaceManager);
            var revisionNumberNode = xmlDoc.SelectSingleNode("//upd:UpdateIdentity/@RevisionNumber", namespaceManager); // Fetch the RevisionNumber

            if (updateIdNode != null && revisionNumberNode != null) // Check if both nodes are not null
            {
                var id = Guid.Parse(updateIdNode.Value);
                int revisionNumber = int.Parse(revisionNumberNode.Value); // Parse the RevisionNumber to int
                updateMetadata.UpdateID = new MicrosoftUpdatePackageIdentity(id, revisionNumber); // Use the fetched RevisionNumber
            }


            var defaultPropertiesLanguageNode = xmlDoc.SelectSingleNode("//upd:Properties/@DefaultPropertiesLanguage", namespaceManager);
            if (defaultPropertiesLanguageNode != null)
            {
                updateMetadata.DefaultPropertiesLanguage = defaultPropertiesLanguageNode.Value;
            }

            var updateTypeNode = xmlDoc.SelectSingleNode("//upd:Properties/@UpdateType", namespaceManager);
            if (updateTypeNode != null)
            {
                updateMetadata.UpdateType = updateTypeNode.Value;
            }

            var maxDownloadSizeNode = xmlDoc.SelectSingleNode("//upd:Properties/@MaxDownloadSize", namespaceManager);
            if (maxDownloadSizeNode != null)
            {
                updateMetadata.MaxDownloadSize = long.Parse(maxDownloadSizeNode.Value); // Use long.Parse instead of int.Parse
            }

            var minDownloadSizeNode = xmlDoc.SelectSingleNode("//upd:Properties/@MinDownloadSize", namespaceManager);
            if (minDownloadSizeNode != null)
            {
                updateMetadata.MinDownloadSize = long.Parse(minDownloadSizeNode.Value); // Use long.Parse instead of int.Parse
            }

            var publicationStateNode = xmlDoc.SelectSingleNode("//upd:Properties/@PublicationState", namespaceManager);
            if (publicationStateNode != null)
            {
                updateMetadata.PublicationState = publicationStateNode.Value;
            }

            var creationDateNode = xmlDoc.SelectSingleNode("//upd:Properties/@CreationDate", namespaceManager);
            if (creationDateNode != null)
            {
                updateMetadata.CreationDate = DateTime.Parse(creationDateNode.Value, null, System.Globalization.DateTimeStyles.RoundtripKind);
            }

            var publisherIdNode = xmlDoc.SelectSingleNode("//upd:Properties/@PublisherID", namespaceManager);
            if (publisherIdNode != null)
            {
                updateMetadata.PublisherID = Guid.Parse(publisherIdNode.Value);
            }

            var titleNode = xmlDoc.SelectSingleNode("//upd:LocalizedProperties/upd:Title", namespaceManager);
            if (titleNode != null)
            {
                updateMetadata.Title = titleNode.InnerText;
            }

            return updateMetadata;
        }
    }
}
