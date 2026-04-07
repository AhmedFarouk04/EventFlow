using MediatR;

namespace EventDrivenBookingPlatform.Modules.Reservations.Application.DTOs;

public record CreateReservationItemDto(string Name, int Quantity, decimal UnitPrice);
