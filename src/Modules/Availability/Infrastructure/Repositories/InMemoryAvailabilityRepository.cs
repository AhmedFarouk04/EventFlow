using EventDrivenBookingPlatform.Modules.Availability.Application.Interfaces;
using EventDrivenBookingPlatform.Modules.Availability.Domain.Aggregates;

namespace EventDrivenBookingPlatform.Modules.Availability.Infrastructure.Repositories;

public class InMemoryAvailabilityRepository : IAvailabilityRepository
{
    private readonly List<AvailabilitySlot> _slots = [];
    private readonly object _sync = new();

    public Task<AvailabilitySlot?> FindAsync(Guid serviceId, DateTime date, CancellationToken cancellationToken = default)
    {
        lock (_sync)
        {
            return Task.FromResult(_slots.FirstOrDefault(s => s.ServiceId == serviceId && s.Date == date.Date));
        }
    }

    public Task<AvailabilitySlot> GetOrCreateAsync(Guid serviceId, DateTime date, CancellationToken cancellationToken = default)
    {
        lock (_sync)
        {
            var slot = _slots.FirstOrDefault(s => s.ServiceId == serviceId && s.Date == date.Date);
            if (slot is null)
            {
                slot = new AvailabilitySlot(serviceId, date, 10);
                _slots.Add(slot);
            }

            return Task.FromResult(slot);
        }
    }

    public Task<IReadOnlyCollection<AvailabilitySlot>> GetByServiceRangeAsync(Guid serviceId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        lock (_sync)
        {
            var result = _slots
                .Where(s => s.ServiceId == serviceId && s.Date >= fromDate.Date && s.Date <= toDate.Date)
                .OrderBy(s => s.Date)
                .ToList();
            return Task.FromResult((IReadOnlyCollection<AvailabilitySlot>)result);
        }
    }

    public Task SaveAsync(AvailabilitySlot slot, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
