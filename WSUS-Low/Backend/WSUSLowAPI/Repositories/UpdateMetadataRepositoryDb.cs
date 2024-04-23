using WSUSLowAPI.Contexts;
using WSUSLowAPI.Models;

namespace WSUSLowAPI.Repositories
{
    public class UpdateMetadataRepositoryDb(WSUSDbContext dbcontext) : IUpdateMetadataRepository
    {
        public List<UpdateMetadata> GetAll()
        {
            return [.. dbcontext.UpdateData];
        }
    }
}
