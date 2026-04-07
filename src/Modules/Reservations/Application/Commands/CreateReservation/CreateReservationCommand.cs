using EventDrivenBookingPlatform.Modules.Reservations.Domain.ValueObjects;
using MediatR;

namespace EventDrivenBookingPlatform.Modules.Reservations.Application.Commands.CreateReservation;

public record CreateReservationCommand(
    Guid CustomerId,
    Guid ServiceId,
    DateTime StartDate,
    DateTime EndDate,
    IReadOnlyCollection<CreateReservationItemRequest>? Items) : IRequest<Guid>;

public record CreateReservationItemRequest(string Name, int Quantity, decimal UnitPrice);
