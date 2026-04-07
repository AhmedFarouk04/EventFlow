using EventDrivenBookingPlatform.Modules.Pricing.Domain;
using EventDrivenBookingPlatform.Modules.Pricing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventDrivenBookingPlatform.Modules.Pricing.Infrastructure.DependencyInjection;

public class PricingSeedService
{
    private readonly PricingDbContext _context;

    public PricingSeedService(PricingDbContext context)
    {
        _context = context;
    }

    public async Task SeedDefaultRuleAsync(CancellationToken cancellationToken = default)
    {
        var exists = await _context.PricingRules.AnyAsync(x => x.ServiceId == Guid.Empty, cancellationToken);
        if (exists)
        {
            return;
        }

        await _context.PricingRules.AddAsync(new PricingRule(Guid.Empty, 100m, 1.2m, 15m), cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
