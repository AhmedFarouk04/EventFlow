using EventDrivenBookingPlatform.BuildingBlocks.SharedKernel;
using MediatR;

namespace EventDrivenBookingPlatform.Modules.Reservations.Application.Commands.CreateReservation;

public record CreateReservationCommand(
    string FullName,
    string Email,
    string PhoneNumber,
    string Nationality,
    string? PassportNumber,
    bool IsDomestic,
    Guid TripId,
    string TripName,
    string Destination,
    int Duration,
    decimal BasePrice,
    decimal Discounts,
    string Currency,
    DateTime CheckInDate,
    DateTime CheckOutDate,
    int NumberOfGuests,
    string? SpecialRequests
) : IRequest<Result<Guid>>;