using EventDrivenBookingPlatform.Modules.Pricing.Application.Interfaces;
using EventDrivenBookingPlatform.Modules.Pricing.Infrastructure.Persistence;

namespace EventDrivenBookingPlatform.Modules.Pricing.Infrastructure.Stores;

public class PriceCalculationStore : IPriceCalculationStore
{
    private readonly PricingDbContext _context;

    public PriceCalculationStore(PricingDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Guid reservationId, Guid serviceId, decimal basePrice, decimal totalPrice, int numberOfDays, CancellationToken cancellationToken = default)
    {
        await _context.PriceCalculations.AddAsync(new PriceCalculationRecord
        {
            Id = Guid.NewGuid(),
            ReservationId = reservationId,
            ServiceId = serviceId,
            BasePrice = basePrice,
            TotalPrice = totalPrice,
            NumberOfDays = numberOfDays,
            CalculatedAt = DateTime.UtcNow
        }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
