using EventDrivenBookingPlatform.BuildingBlocks.SharedKernel;
using EventDrivenBookingPlatform.Modules.Reservations.Domain.Entities;
using EventDrivenBookingPlatform.Modules.Reservations.Domain.Events;
using EventDrivenBookingPlatform.Modules.Reservations.Domain.Rules;
using EventDrivenBookingPlatform.Modules.Reservations.Domain.ValueObjects;

namespace EventDrivenBookingPlatform.Modules.Reservations.Domain.Aggregates;

public class Reservation : AggregateRoot
{
    public CustomerId CustomerId { get; private set; } = null!;
    public ServiceId ServiceId { get; private set; } = null!;
    public DateRange DateRange { get; private set; } = null!;
    public ReservationStatus Status { get; private set; }
    public List<ReservationItem> Items { get; private set; } = new();

    private Reservation()
    {
    }

    private Reservation(ReservationId id, CustomerId customerId, ServiceId serviceId, DateRange dateRange)
    {
        Id = id.Value;
        CustomerId = customerId;
        ServiceId = serviceId;
        DateRange = dateRange;
        Status = ReservationStatus.Pending;

        AddDomainEvent(new ReservationCreatedEvent(Id, customerId.Value, serviceId.Value, dateRange.StartDate, dateRange.EndDate));
    }

    public static Result<Reservation> Create(ReservationId id, CustomerId customerId, ServiceId serviceId, DateRange dateRange)
    {
        var dateRangeRule = new ReservationMustHaveValidDateRangeRule(dateRange);
        if (dateRangeRule.IsBroken())
        {
            return Result<Reservation>.Failure(dateRangeRule.Message);
        }

        if (dateRange.StartDate < DateTime.UtcNow.Date)
        {
            return Result<Reservation>.Failure("Reservation start date cannot be in the past.");
        }

        return Result<Reservation>.Success(new Reservation(id, customerId, serviceId, dateRange));
    }

    public Result Confirm()
    {
        if (Status != ReservationStatus.Pending)
        {
            return Result.Failure("Only pending reservations can be confirmed.");
        }

        Status = ReservationStatus.Confirmed;
        AddDomainEvent(new ReservationConfirmedEvent(Id, CustomerId.Value, ServiceId.Value));

        return Result.Success();
    }

    public Result Cancel(string reason)
    {
        if (Status == ReservationStatus.Cancelled)
        {
            return Result.Failure("Reservation is already cancelled.");
        }

        Status = ReservationStatus.Cancelled;
        AddDomainEvent(new ReservationCancelledEvent(Id, CustomerId.Value, ServiceId.Value, reason));

        return Result.Success();
    }

    public Result AddItem(string name, int quantity, decimal unitPrice)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure("Item name is required.");
        }

        if (quantity <= 0)
        {
            return Result.Failure("Item quantity must be greater than zero.");
        }

        if (unitPrice < 0)
        {
            return Result.Failure("Item unit price cannot be negative.");
        }

        Items.Add(ReservationItem.Create(name, quantity, unitPrice));
        return Result.Success();
    }
}
