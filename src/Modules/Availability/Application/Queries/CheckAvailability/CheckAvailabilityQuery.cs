using MediatR;

namespace EventDrivenBookingPlatform.Modules.Availability.Application.Queries.CheckAvailability;

public record CheckAvailabilityQuery(Guid ServiceId, DateTime Date, int Quantity) : IRequest<bool>;
