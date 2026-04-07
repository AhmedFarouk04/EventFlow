using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EventDrivenBookingPlatform.Modules.Availability.Infrastructure.Persistence;

public class AvailabilityDbContextFactory : IDesignTimeDbContextFactory<AvailabilityDbContext>
{
    public AvailabilityDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AvailabilityDbContext>();
        optionsBuilder.UseSqlServer(
            "Server=(localdb)\\mssqllocaldb;Database=EventDrivenBookingAvailabilityDb;Trusted_Connection=True;MultipleActiveResultSets=true");

        return new AvailabilityDbContext(optionsBuilder.Options);
    }
}
