using EventDrivenBookingPlatform.Modules.Reservations.Application.Interfaces;
using EventDrivenBookingPlatform.Modules.Reservations.Domain.Aggregates;
using EventDrivenBookingPlatform.Modules.Reservations.Domain.ValueObjects;
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

    public async Task<Reservation?> GetByIdAsync(ReservationId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Reservations
            .Include(r => r.Items)
            .FirstOrDefaultAsync(r => r.Id == id.Value, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Reservation>> GetByCustomerIdAsync(CustomerId customerId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Reservations
            .Include(r => r.Items)
            .Where(r => r.CustomerId == customerId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasOverlapAsync(CustomerId customerId, DateRange dateRange, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Reservations
            .Where(r => r.CustomerId == customerId && r.Status != ReservationStatus.Cancelled)
            .AnyAsync(r =>
                dateRange.StartDate < r.DateRange.EndDate &&
                dateRange.EndDate > r.DateRange.StartDate,
                cancellationToken);
    }

    public async Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default)
    {
        await _dbContext.Reservations.AddAsync(reservation, cancellationToken);
    }

    public Task UpdateAsync(Reservation reservation, CancellationToken cancellationToken = default)
    {
        _dbContext.Reservations.Update(reservation);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
