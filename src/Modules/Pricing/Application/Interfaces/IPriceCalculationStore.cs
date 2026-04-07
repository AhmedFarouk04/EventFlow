namespace EventDrivenBookingPlatform.Modules.Pricing.Application.Interfaces;

public interface IPriceCalculationStore
{
    Task AddAsync(Guid reservationId, Guid serviceId, decimal basePrice, decimal totalPrice, int numberOfDays, CancellationToken cancellationToken = default);
}
