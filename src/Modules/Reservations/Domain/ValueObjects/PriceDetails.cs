using EventDrivenBookingPlatform.BuildingBlocks.SharedKernel;

namespace EventDrivenBookingPlatform.Modules.Reservations.Domain.ValueObjects;

public class PriceDetails : ValueObject
{
    private const decimal TaxRate = 0.145m;

    public decimal BasePrice { get; }
    public decimal Taxes { get; }
    public decimal Discounts { get; }
    public decimal TotalPrice { get; }
    public string Currency { get; }

    private PriceDetails(decimal basePrice, decimal discounts, string currency)
    {
        BasePrice = Math.Round(basePrice, 2);
        Taxes = Math.Round(basePrice * TaxRate, 2);
        Discounts = Math.Round(discounts, 2);
        TotalPrice = BasePrice + Taxes - Discounts;
        Currency = currency.ToUpperInvariant();
    }

    public static Result<PriceDetails> Create(decimal basePrice, decimal discounts = 0, string currency = "USD")
    {
        if (basePrice <= 0 || basePrice > 100000) return Result<PriceDetails>.Failure("Base price must be between 0 and 100,000.");
        if (discounts < 0 || discounts > basePrice) return Result<PriceDetails>.Failure("Discounts cannot be negative or exceed the base price.");
        if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3) return Result<PriceDetails>.Failure("Currency must be a valid 3-letter ISO 4217 code.");

        return Result<PriceDetails>.Success(new PriceDetails(basePrice, discounts, currency));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return BasePrice;
        yield return Taxes;
        yield return Discounts;
        yield return TotalPrice;
        yield return Currency;
    }
}