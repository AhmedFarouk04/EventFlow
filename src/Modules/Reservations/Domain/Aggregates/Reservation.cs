using EventDrivenBookingPlatform.BuildingBlocks.SharedKernel;
using EventDrivenBookingPlatform.Modules.Reservations.Domain.Events;
using EventDrivenBookingPlatform.Modules.Reservations.Domain.ValueObjects;

namespace EventDrivenBookingPlatform.Modules.Reservations.Domain.Aggregates;

public class Reservation : AggregateRoot
{
    public CustomerInfo CustomerInfo { get; private set; }
    public TripDetails TripDetails { get; private set; }
    public PriceDetails PriceDetails { get; private set; }
    public ReservationStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime LastModified { get; private set; }
    public DateTime CheckInDate { get; private set; }
    public DateTime CheckOutDate { get; private set; }
    public int NumberOfGuests { get; private set; }
    public string? SpecialRequests { get; private set; }

    private Reservation() { } // For EF Core

    private Reservation(
        Guid id,
        CustomerInfo customerInfo,
        TripDetails tripDetails,
        PriceDetails priceDetails,
        DateTime checkInDate,
        DateTime checkOutDate,
        int numberOfGuests,
        string? specialRequests)
    {
        Id = id;
        CustomerInfo = customerInfo;
        TripDetails = tripDetails;
        PriceDetails = priceDetails;
        CheckInDate = checkInDate;
        CheckOutDate = checkOutDate;
        NumberOfGuests = numberOfGuests;
        SpecialRequests = specialRequests;
        Status = ReservationStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        LastModified = DateTime.UtcNow;

        AddDomainEvent(new ReservationCreatedDomainEvent(
            Id,
            CustomerInfo.Email,
            CheckInDate,
            CheckOutDate,
            PriceDetails.TotalPrice));
    }

    public static Result<Reservation> Create(
        CustomerInfo customerInfo,
        TripDetails tripDetails,
        PriceDetails priceDetails,
        DateTime checkInDate,
        DateTime checkOutDate,
        int numberOfGuests,
        string? specialRequests,
        DateTime currentDate)
    {
        if (checkInDate < currentDate.AddHours(24))
            return Result<Reservation>.Failure("Reservations must be made at least 24 hours in advance.");

        if (checkInDate > currentDate.AddYears(1))
            return Result<Reservation>.Failure("Cannot book more than 1 year in advance.");

        if (checkOutDate <= checkInDate)
            return Result<Reservation>.Failure("Check-out date must be after check-in date.");

        if ((checkOutDate - checkInDate).Days > 30)
            return Result<Reservation>.Failure("Maximum stay duration is 30 days.");

        if (numberOfGuests < 1)
            return Result<Reservation>.Failure("At least 1 guest is required.");

        if (specialRequests?.Length > 500)
            return Result<Reservation>.Failure("Special requests cannot exceed 500 characters.");

        var reservation = new Reservation(
            Guid.NewGuid(),
            customerInfo,
            tripDetails,
            priceDetails,
            checkInDate,
            checkOutDate,
            numberOfGuests,
            specialRequests);

        return Result<Reservation>.Success(reservation);
    }

    public Result ConfirmPayment()
    {
        if (Status != ReservationStatus.Pending)
            return Result.Failure("Only pending reservations can be confirmed.");

        Status = ReservationStatus.Confirmed;
        LastModified = DateTime.UtcNow;

        return Result.Success();
    }
}