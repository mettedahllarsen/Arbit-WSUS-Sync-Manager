using Microsoft.EntityFrameworkCore;
using WSUSLowAPI.Models;

namespace WSUSLowAPI.Contexts
{
    public class WSUSDbContext(DbContextOptions<WSUSDbContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UpdateData>().Property(p => p.UpdateID).HasColumnName("UpdateID");
        }
        public DbSet<UpdateData> UpdateMetadata { get; set; }

    }
}
