using WSUSLowAPI.Models;

namespace WSUSLowAPI.Repositories
{
    public interface IUpdateMetadataRepository
    {
        List<UpdateMetadata> GetAll();
    }
}
