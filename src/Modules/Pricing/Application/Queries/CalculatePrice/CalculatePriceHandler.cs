using EventDrivenBookingPlatform.Modules.Pricing.Domain;
using EventDrivenBookingPlatform.Modules.Pricing.Application.Interfaces;
using MediatR;

namespace EventDrivenBookingPlatform.Modules.Pricing.Application.Queries.CalculatePrice;

public class CalculatePriceHandler : IRequestHandler<CalculatePriceQuery, PriceCalculationDto>
{
    private readonly IPricingRuleRepository _pricingRuleRepository;

    public CalculatePriceHandler(IPricingRuleRepository pricingRuleRepository)
    {
        _pricingRuleRepository = pricingRuleRepository;
    }

    public async Task<PriceCalculationDto> Handle(CalculatePriceQuery request, CancellationToken cancellationToken)
    {
        var days = Math.Max(1, (request.EndDate.Date - request.StartDate.Date).Days);
        var seasonalMultiplier = request.StartDate.Month is 6 or 7 or 8 ? 1.2m : 1m;
        var rule = await _pricingRuleRepository.GetByServiceIdAsync(request.ServiceId, cancellationToken)
            ?? new PricingRule(request.ServiceId, basePrice: 100m, seasonalMultiplier: seasonalMultiplier, serviceFee: 15m);
        var total = rule.CalculateTotal(days);

        return new PriceCalculationDto(rule.BasePrice, total, days);
    }
}
