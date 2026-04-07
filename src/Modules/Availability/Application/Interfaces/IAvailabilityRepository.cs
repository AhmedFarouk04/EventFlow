using EventDrivenBookingPlatform.Modules.Availability.Domain.Aggregates;

namespace EventDrivenBookingPlatform.Modules.Availability.Application.Interfaces;

public interface IAvailabilityRepository
{
    Task<AvailabilitySlot?> FindAsync(Guid serviceId, DateTime date, CancellationToken cancellationToken = default);
    Task<AvailabilitySlot> GetOrCreateAsync(Guid serviceId, DateTime date, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<AvailabilitySlot>> GetByServiceRangeAsync(Guid serviceId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task SaveAsync(AvailabilitySlot slot, CancellationToken cancellationToken = default);
}
