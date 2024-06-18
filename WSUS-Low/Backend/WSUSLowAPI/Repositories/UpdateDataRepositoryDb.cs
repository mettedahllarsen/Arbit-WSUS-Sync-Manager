using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Microsoft.PackageGraph.MicrosoftUpdate.Metadata;
using Microsoft.PackageGraph.MicrosoftUpdate.Source;
using Microsoft.PackageGraph.Storage;
using Microsoft.PackageGraph.Storage.Local;
using System.Xml;
using WSUSLowAPI.Models;
using Endpoint = Microsoft.PackageGraph.MicrosoftUpdate.Source.Endpoint;

namespace WSUSLowAPI.Repositories
{
    public class UpdateDataRepositoryDb : IUpdateDataRepository
    {
        private readonly string _connectionString = "server=localhost;database=WSUSLowDB;" + "user id=wsusmikkel;password=wsus;TrustServerCertificate=True";

        public IEnumerable<UpdateData> GetAll()
        {
            List<UpdateData> updateDatas = new();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Updates", connection);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string? identifier = reader["UpdateIdentifier"].ToString();
                    byte[] publisherID = (byte[])reader["PublisherId"];
                    Guid publisherGuid = new(publisherID);
                    UpdateData updateData = new()
                    {
                        UpdateID = Convert.ToInt32(reader["ComputerID"]),
                        UpdateIdentifier = MicrosoftUpdatePackageIdentity.FromString(identifier),
                        RevisionNumber = Convert.ToInt32(reader["RevisionNumber"]),
                        UpdateType = reader["UpdateType"].ToString(),
                        MaxDownloadSize = Convert.ToInt32(reader["MaxDownloadSize"]),
                        MinDownloadSize = Convert.ToInt32(reader["MinDownloadSize"]),
                        PublicationState = reader["PublicationState"].ToString(),
                        CreationDate = Convert.ToDateTime(reader["CreationDate"]),
                        PublisherID = publisherGuid,
                        Title = reader["Title"].ToString()
                    };
                    updateDatas.Add(updateData);
                }
            }
            return updateDatas;
        }

        public string FetchToDb(string? filter)
        {
            GetAvailableUpdates(filter);
            InsertIntoDatabase();
            return "Metadata added to the Database";
        }

        private static void GetAvailableUpdates(string? filter)
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
            } else
            {
                Console.WriteLine("Fetching all updates from upstream and saving them to the local store...");
            }
            UpstreamUpdatesSource updatesSource = new(Endpoint.Default, updatesFilter);
            updatesSource.MetadataCopyProgress += PackageStore_MetadataCopyProgress;


            var timeout = TimeSpan.FromMinutes(20);

            using (var cts = new CancellationTokenSource(timeout))
            {
                Console.WriteLine("Starting copy operation with a timeout of 20 minutes...");
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

            var foundMetadatas = GetAll();

            foreach (string  xmlFile in xmlFiles)
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlFile);

                var updateMetadata = ParseMetadataFromXml(xmlDoc);
                //updateMetadata.Validate(); Not working yet

                bool skipped = false;

                var foundMetadata = foundMetadatas.FirstOrDefault(x => x.UpdateID == updateMetadata.UpdateID && x.RevisionNumber == updateMetadata.RevisionNumber);
                if (foundMetadata != null)
                {
                    _dbContext.UpdateMetadata.Add(updateMetadata);
                    _dbContext.SaveChanges();
                    successfulInserts++;
                } else
                {
                    skipped = true;
                }

                processedFiles++;
                double progressPercentage = (double)processedFiles / totalFiles * 100;
                string progressMessage = skipped ? $"Progress: {progressPercentage:F2}% ({processedFiles}/{totalFiles}) - Skipping" : $"Progress: {progressPercentage:F2}% ({processedFiles}/{totalFiles})";
                Console.Write($"\r{progressMessage}");
            }

            Console.WriteLine($"Successfully inserted {successfulInserts} updates into the database.");
        }

        private static UpdateData ParseMetadataFromXml(XmlDocument xmlDoc)
        {
            var updateMetadata = new UpdateData();

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
