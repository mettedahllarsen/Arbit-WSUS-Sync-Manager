//using WSUSLowAPI.Models;

//namespace WSUSLowAPI.Repositories
//{
//    public class UpdateMetadataRepository : IUpdateMetadataRepository
//    {
//        public List<UpdateMetadata> _data;
//        public int nextId;

//        public UpdateMetadataRepository()
//        {
//            nextId = 1;
//            _data =
//            [
//                new() {
//                    MetadataId = nextId++,
//                    UpdateID = null,
//                    RevisionNumber = 5,
//                    DefaultPropertiesLanguage = "en-US",
//                    UpdateType = "Security",
//                    MaxDownloadSize = 102400,
//                    MinDownloadSize = 51200,
//                    PublicationState = "Published",
//                    CreationDate = DateTime.Now,
//                    PublisherID = Guid.NewGuid(),
//                    Title = "Security Update for Windows 10"
//                },
//                new()
//                {
//                    MetadataId = nextId++,
//                    UpdateID = null,
//                    RevisionNumber = 3,
//                    DefaultPropertiesLanguage = "fr-FR",
//                    UpdateType = "Feature",
//                    MaxDownloadSize = 204800,
//                    MinDownloadSize = 102400,
//                    PublicationState = "Draft",
//                    CreationDate = DateTime.Now.AddDays(-30),
//                    PublisherID = Guid.NewGuid(),
//                    Title = "Feature Update for Windows 11"
//                }
//            ];
//        }

//        public List<UpdateMetadata> GetAll()
//        {
//            return new List<UpdateMetadata>(_data);
//        }

//        public string FetchToDb(string? filter)
//        {
//            return "Not implemented";
//        }
//    }
//}
