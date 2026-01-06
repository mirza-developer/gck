using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Gck.Persistence;

public class GckDbContextFactory : IDesignTimeDbContextFactory<GckDbContext>
{
    public GckDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<GckDbContext>();
        optionsBuilder.UseSqlServer("Server=.;Database=GckDb;Trusted_Connection=True;TrustServerCertificate=True;");

        return new GckDbContext(optionsBuilder.Options);
    }
}
