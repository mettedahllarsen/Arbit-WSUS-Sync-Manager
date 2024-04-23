using Microsoft.EntityFrameworkCore;
using WSUSLowAPI.Models;

namespace WSUSLowAPI.Contexts
{
    public class WSUSDbContext(DbContextOptions<WSUSDbContext> options) : DbContext(options)
    {
        // Define DbSet properties for each table you want to interact with
        public DbSet<UpdateMetadata> UpdateMetadata { get; set; }

    }
}
