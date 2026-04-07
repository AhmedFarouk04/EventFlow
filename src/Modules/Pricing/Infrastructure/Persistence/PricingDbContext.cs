using EventDrivenBookingPlatform.Modules.Pricing.Domain;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace EventDrivenBookingPlatform.Modules.Pricing.Infrastructure.Persistence;

public class PricingDbContext : DbContext
{
    public PricingDbContext(DbContextOptions<PricingDbContext> options) : base(options)
    {
    }

    public DbSet<PricingRule> PricingRules => Set<PricingRule>();
    public DbSet<PriceCalculationRecord> PriceCalculations => Set<PriceCalculationRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
