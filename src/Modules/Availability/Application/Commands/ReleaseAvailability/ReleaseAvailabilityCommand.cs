using MediatR;

namespace EventDrivenBookingPlatform.Modules.Availability.Application.Commands.ReleaseAvailability;

public record ReleaseAvailabilityCommand(Guid ServiceId, DateTime Date, int Quantity) : IRequest;
