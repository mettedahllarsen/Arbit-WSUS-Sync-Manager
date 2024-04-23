using WSUSLowAPI.Contexts;

namespace WSUSLowAPI.Repositories
{
    public class UpdateDataRepositoryDb : IUpdateDataRepository
    {
        private readonly WSUSDbContext _context;

        public UpdateDataRepositoryDb(WSUSDbContext dbcontext)
        {
            _context = dbcontext;
        }
    }
}
