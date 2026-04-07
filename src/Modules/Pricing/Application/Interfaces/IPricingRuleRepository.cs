using EventDrivenBookingPlatform.Modules.Pricing.Domain;

namespace EventDrivenBookingPlatform.Modules.Pricing.Application.Interfaces;

public interface IPricingRuleRepository
{
    Task<PricingRule?> GetByServiceIdAsync(Guid serviceId, CancellationToken cancellationToken = default);
}
