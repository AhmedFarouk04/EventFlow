using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EventDrivenBookingPlatform.Modules.Pricing.Infrastructure.Persistence;

public class PricingDbContextFactory : IDesignTimeDbContextFactory<PricingDbContext>
{
    public PricingDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PricingDbContext>();
        optionsBuilder.UseSqlServer(
            "Server=(localdb)\\mssqllocaldb;Database=EventDrivenBookingPricingDb;Trusted_Connection=True;MultipleActiveResultSets=true");

        return new PricingDbContext(optionsBuilder.Options);
    }
}
