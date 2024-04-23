using WSUSLowAPI.Models;

namespace WSUSLowAPI.Repositories
{
    public class UpdateDataRepository : IUpdateDataRepository
    {
        public List<UpdateData> _data;
        public int nextId;

        public UpdateDataRepository()
        {
            nextId = 1;
            _data =
            [
                new() {
                    Id = nextId++,
                    UpdateID = Guid.NewGuid(),
                    RevisionNumber = 5,
                    DefaultPropertiesLanguage = "en-US",
                    UpdateType = "Security",
                    MaxDownloadSize = 102400,
                    MinDownloadSize = 51200,
                    PublicationState = "Published",
                    CreationDate = DateTime.Now,
                    PublisherID = Guid.NewGuid(),
                    Title = "Security Update for Windows 10"
                },
                new()
                {
                    Id = nextId++,
                    UpdateID = Guid.NewGuid(),
                    RevisionNumber = 3,
                    DefaultPropertiesLanguage = "fr-FR",
                    UpdateType = "Feature",
                    MaxDownloadSize = 204800,
                    MinDownloadSize = 102400,
                    PublicationState = "Draft",
                    CreationDate = DateTime.Now.AddDays(-30),
                    PublisherID = Guid.NewGuid(),
                    Title = "Feature Update for Windows 11"
                }
            ];
        }

        public List<UpdateData> GetAll()
        {
            return new List<UpdateData>(_data);
        }
    }
}
