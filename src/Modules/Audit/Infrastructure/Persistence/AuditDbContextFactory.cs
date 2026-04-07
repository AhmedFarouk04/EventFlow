using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EventDrivenBookingPlatform.Modules.Audit.Infrastructure.Persistence;

public class AuditDbContextFactory : IDesignTimeDbContextFactory<AuditDbContext>
{
    public AuditDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AuditDbContext>();
        optionsBuilder.UseSqlServer(
            "Server=(localdb)\\mssqllocaldb;Database=EventDrivenBookingAuditDb;Trusted_Connection=True;MultipleActiveResultSets=true");

        return new AuditDbContext(optionsBuilder.Options);
    }
}
