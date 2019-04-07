using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace StarOJ.Data.Provider.SqlServer.Models
{
    public class OJContext : DbContext
    {
        public OJContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Problem> Problems { get; set; }

        public DbSet<Submission> Submissions { get; set; }

        public DbSet<TestCase> Tests { get; set; }

        public DbSet<WorkspaceInfo> WorkspaceInfos { get; set; }
    }

    public class OJContextFactory : IDesignTimeDbContextFactory<OJContext>
    {
        public OJContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<OJContext> optionsBuilder = new DbContextOptionsBuilder<OJContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=StarOJ-db;Trusted_Connection=True;MultipleActiveResultSets=true");

            return new OJContext(optionsBuilder.Options);
        }
    }
}
