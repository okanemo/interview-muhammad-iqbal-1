using Microsoft.EntityFrameworkCore;
using Testing2.Models;

namespace Testing2
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext()
        {
        }

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }
        public virtual DbSet<MasterMenu> MasterMenu { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<AccessMenu> AccessMenu { get; set; }
        public virtual DbSet<MasterRole> MasterRole { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
