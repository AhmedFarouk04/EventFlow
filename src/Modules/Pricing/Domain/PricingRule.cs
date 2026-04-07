namespace EventDrivenBookingPlatform.Modules.Pricing.Domain;

public class PricingRule
{
    public Guid Id { get; private set; }
    public Guid ServiceId { get; private set; }
    public decimal BasePrice { get; private set; }
    public decimal SeasonalMultiplier { get; private set; }
    public decimal ServiceFee { get; private set; }

    private PricingRule() { }

    public PricingRule(Guid serviceId, decimal basePrice, decimal seasonalMultiplier, decimal serviceFee)
    {
        Id = Guid.NewGuid();
        ServiceId = serviceId;
        BasePrice = basePrice;
        SeasonalMultiplier = seasonalMultiplier;
        ServiceFee = serviceFee;
    }

    public decimal CalculateTotal(int days)
    {
        return Math.Round((BasePrice * days * SeasonalMultiplier) + ServiceFee, 2);
    }
}
