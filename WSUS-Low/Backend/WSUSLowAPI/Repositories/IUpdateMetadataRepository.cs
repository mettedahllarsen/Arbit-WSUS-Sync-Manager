using WSUSLowAPI.Models;

namespace WSUSLowAPI.Repositories
{
    public interface IUpdateMetadataRepository
    {
        IEnumerable<UpdateMetadata> GetAll();
        string FetchToDb(string? titleFilter);
    }
}
