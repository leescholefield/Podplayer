using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Podplayer.Entity
{
    public class PodplayerDbContextFactory : IDesignTimeDbContextFactory<PodplayerDbContext>
    {
        public PodplayerDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<PodplayerDbContext>();
            options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=PodplayerDb-1;Trusted_Connection=True;MultipleActiveResultSets=true");

            return new PodplayerDbContext(options.Options);
        }
    }
}
