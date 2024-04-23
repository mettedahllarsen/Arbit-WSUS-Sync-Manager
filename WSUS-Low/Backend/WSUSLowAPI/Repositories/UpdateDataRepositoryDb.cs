using WSUSLowAPI.Contexts;
using WSUSLowAPI.Models;

namespace WSUSLowAPI.Repositories
{
    public class UpdateDataRepositoryDb : IUpdateDataRepository
    {
        private readonly WSUSDbContext _context;

        public UpdateDataRepositoryDb(WSUSDbContext dbcontext)
        {
            _context = dbcontext;
        }

        public List<UpdateData> GetAll()
        {
            return [.. _context.UpdateData];
        }
    }
}
