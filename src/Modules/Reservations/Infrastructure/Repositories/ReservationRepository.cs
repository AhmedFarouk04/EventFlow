using EventDrivenBookingPlatform.Modules.Reservations.Application.Interfaces;
using EventDrivenBookingPlatform.Modules.Reservations.Domain.Aggregates;
using EventDrivenBookingPlatform.Modules.Reservations.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventDrivenBookingPlatform.Modules.Reservations.Infrastructure.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly ReservationsDbContext _dbContext;

    public ReservationRepository(ReservationsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default)
    {
        await _dbContext.Reservations.AddAsync(reservation, cancellationToken);
    }

    public async Task<bool> IsOverlappingAsync(string customerEmail, DateTime checkInDate, DateTime checkOutDate, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Reservations
            .AnyAsync(r =>
                r.CustomerInfo.Email == customerEmail &&
                r.Status != ReservationStatus.Cancelled &&
                r.Status != ReservationStatus.Declined &&
                ((checkInDate >= r.CheckInDate && checkInDate < r.CheckOutDate) ||
                 (checkOutDate > r.CheckInDate && checkOutDate <= r.CheckOutDate) ||
                 (checkInDate <= r.CheckInDate && checkOutDate >= r.CheckOutDate)),
                cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    public async Task<Reservation?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Reservations
            .FirstOrDefaultAsync(r => r.Id == id);
    }
}