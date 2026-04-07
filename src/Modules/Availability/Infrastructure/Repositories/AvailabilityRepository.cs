using EventDrivenBookingPlatform.Modules.Availability.Application.Interfaces;
using EventDrivenBookingPlatform.Modules.Availability.Domain.Aggregates;
using EventDrivenBookingPlatform.Modules.Availability.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventDrivenBookingPlatform.Modules.Availability.Infrastructure.Repositories;

public class AvailabilityRepository : IAvailabilityRepository
{
    private readonly AvailabilityDbContext _context;

    public AvailabilityRepository(AvailabilityDbContext context)
    {
        _context = context;
    }

    public Task<AvailabilitySlot?> FindAsync(Guid serviceId, DateTime date, CancellationToken cancellationToken = default)
    {
        return _context.AvailabilitySlots
            .FirstOrDefaultAsync(x => x.ServiceId == serviceId && x.Date == date.Date, cancellationToken);
    }

    public async Task<AvailabilitySlot> GetOrCreateAsync(Guid serviceId, DateTime date, CancellationToken cancellationToken = default)
    {
        var slot = await FindAsync(serviceId, date, cancellationToken);
        if (slot is not null)
        {
            return slot;
        }

        slot = new AvailabilitySlot(serviceId, date.Date, 10);
        await _context.AvailabilitySlots.AddAsync(slot, cancellationToken);
        return slot;
    }

    public async Task<IReadOnlyCollection<AvailabilitySlot>> GetByServiceRangeAsync(Guid serviceId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        return await _context.AvailabilitySlots
            .Where(x => x.ServiceId == serviceId && x.Date >= fromDate.Date && x.Date <= toDate.Date)
            .OrderBy(x => x.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task SaveAsync(AvailabilitySlot slot, CancellationToken cancellationToken = default)
    {
        if (_context.Entry(slot).State == EntityState.Detached)
        {
            _context.AvailabilitySlots.Attach(slot);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
