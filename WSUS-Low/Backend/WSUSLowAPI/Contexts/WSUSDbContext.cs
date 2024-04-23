using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WSUSLowAPI.Models;

namespace WSUSLowAPI.Contexts
{
    public class WSUSDbContext : DbContext
    {
        public WSUSDbContext(DbContextOptions<WSUSDbContext> options) : base(options)
        {
        }

        // Define DbSet properties for each table you want to interact with
        public DbSet<UpdateData> Updates { get; set; }

    }
}
