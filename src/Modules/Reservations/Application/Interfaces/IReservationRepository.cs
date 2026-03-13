using EventDrivenBookingPlatform.Modules.Reservations.Domain.Aggregates;

namespace EventDrivenBookingPlatform.Modules.Reservations.Application.Interfaces;

public interface IReservationRepository
{
    Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default);
    Task<bool> IsOverlappingAsync(string customerEmail, DateTime checkInDate, DateTime checkOutDate, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<Reservation?> GetByIdAsync(Guid id);
}