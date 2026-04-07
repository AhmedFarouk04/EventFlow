using EventDrivenBookingPlatform.BuildingBlocks.SharedKernel;

namespace EventDrivenBookingPlatform.Modules.Reservations.Domain.Entities;

public class ReservationItem : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    public decimal TotalPrice => Quantity * UnitPrice;

    private ReservationItem()
    {
    }

    private ReservationItem(string name, int quantity, decimal unitPrice)
    {
        Id = Guid.NewGuid();
        Name = name;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public static ReservationItem Create(string name, int quantity, decimal unitPrice)
    {
        return new ReservationItem(name, quantity, unitPrice);
    }
}
