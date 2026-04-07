namespace EventDrivenBookingPlatform.Modules.Pricing.Infrastructure.Persistence;

public class PriceCalculationRecord
{
    public Guid Id { get; set; }
    public Guid ReservationId { get; set; }
    public Guid ServiceId { get; set; }
    public decimal BasePrice { get; set; }
    public decimal TotalPrice { get; set; }
    public int NumberOfDays { get; set; }
    public DateTime CalculatedAt { get; set; }
}
