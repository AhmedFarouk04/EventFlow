using EventDrivenBookingPlatform.BuildingBlocks.SharedKernel;
using EventDrivenBookingPlatform.Modules.Reservations.Domain.Aggregates;
using EventDrivenBookingPlatform.Modules.Reservations.Domain.ValueObjects;
using EventDrivenBookingPlatform.Modules.Reservations.Application.Interfaces;
using MediatR;

namespace EventDrivenBookingPlatform.Modules.Reservations.Application.Commands.CreateReservation;

public class CreateReservationHandler : IRequestHandler<CreateReservationCommand, Result<Guid>>
{
    private readonly IReservationRepository _reservationRepository;

    public CreateReservationHandler(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<Result<Guid>> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
    {
        // 1. Create and Validate Value Objects
        var customerInfoResult = CustomerInfo.Create(
            request.FullName, request.Email, request.PhoneNumber, request.Nationality, request.PassportNumber, request.IsDomestic);
        if (customerInfoResult.IsFailure) return Result<Guid>.Failure(customerInfoResult.Error);

        var tripDetailsResult = TripDetails.Create(
            request.TripId, request.TripName, request.Destination, request.Duration);
        if (tripDetailsResult.IsFailure) return Result<Guid>.Failure(tripDetailsResult.Error);

        var priceDetailsResult = PriceDetails.Create(
            request.BasePrice, request.Discounts, request.Currency);
        if (priceDetailsResult.IsFailure) return Result<Guid>.Failure(priceDetailsResult.Error);

        // 2. Business Rule: Check for Overlapping Reservations (Rule R007)
        var isOverlapping = await _reservationRepository.IsOverlappingAsync(
            request.Email, request.CheckInDate, request.CheckOutDate, cancellationToken);

        if (isOverlapping)
            return Result<Guid>.Failure("Customer already has a reservation during these dates.");

        // 3. Create Reservation Aggregate
        var reservationResult = Reservation.Create(
            customerInfoResult.Value,
            tripDetailsResult.Value,
            priceDetailsResult.Value,
            request.CheckInDate,
            request.CheckOutDate,
            request.NumberOfGuests,
            request.SpecialRequests,
            DateTime.UtcNow);

        if (reservationResult.IsFailure) return Result<Guid>.Failure(reservationResult.Error);

        // 4. Save to Database
        await _reservationRepository.AddAsync(reservationResult.Value, cancellationToken);
        await _reservationRepository.SaveChangesAsync(cancellationToken);

        // Return the created Reservation ID
        return Result<Guid>.Success(reservationResult.Value.Id);
    }
}