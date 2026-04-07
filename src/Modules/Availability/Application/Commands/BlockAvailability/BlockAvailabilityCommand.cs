using MediatR;

namespace EventDrivenBookingPlatform.Modules.Availability.Application.Commands.BlockAvailability;

public record BlockAvailabilityCommand(Guid ServiceId, DateTime Date, int Quantity) : IRequest;
