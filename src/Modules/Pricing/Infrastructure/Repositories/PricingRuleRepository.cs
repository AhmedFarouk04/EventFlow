using EventDrivenBookingPlatform.Modules.Pricing.Application.Interfaces;
using EventDrivenBookingPlatform.Modules.Pricing.Domain;
using EventDrivenBookingPlatform.Modules.Pricing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventDrivenBookingPlatform.Modules.Pricing.Infrastructure.Repositories;

public class PricingRuleRepository : IPricingRuleRepository
{
    private readonly PricingDbContext _context;

    public PricingRuleRepository(PricingDbContext context)
    {
        _context = context;
    }

    public async Task<PricingRule?> GetByServiceIdAsync(Guid serviceId, CancellationToken cancellationToken = default)
    {
        return await _context.PricingRules.FirstOrDefaultAsync(x => x.ServiceId == serviceId, cancellationToken)
            ?? await _context.PricingRules.FirstOrDefaultAsync(x => x.ServiceId == Guid.Empty, cancellationToken);
    }
}
