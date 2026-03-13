namespace EventDrivenBookingPlatform.Modules.Reservations.Application.DTOs;

public record ReservationDto(
    Guid Id,
    string FullName,
    string Email,
    string TripName,
    string Destination,
    decimal TotalPrice,
    string Currency,
    DateTime CheckInDate,
    DateTime CheckOutDate,
    int Status);