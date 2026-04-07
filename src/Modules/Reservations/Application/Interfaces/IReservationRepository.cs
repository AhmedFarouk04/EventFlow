using EventDrivenBookingPlatform.Modules.Reservations.Domain.Aggregates;
using EventDrivenBookingPlatform.Modules.Reservations.Domain.ValueObjects;

namespace EventDrivenBookingPlatform.Modules.Reservations.Application.Interfaces;

public interface IReservationRepository
{
    Task<Reservation?> GetByIdAsync(ReservationId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Reservation>> GetByCustomerIdAsync(CustomerId customerId, CancellationToken cancellationToken = default);
    Task<bool> HasOverlapAsync(CustomerId customerId, DateRange dateRange, CancellationToken cancellationToken = default);
    Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default);
    Task UpdateAsync(Reservation reservation, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
