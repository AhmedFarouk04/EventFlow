using MediatR;

namespace EventDrivenBookingPlatform.Modules.Availability.Application.Queries.GetAvailableSlots;

public record GetAvailableSlotsQuery(Guid ServiceId, DateTime FromDate, DateTime ToDate) : IRequest<IReadOnlyCollection<AvailableSlotDto>>;

public record AvailableSlotDto(DateTime Date, int Remaining);
