using EventDrivenBookingPlatform.BuildingBlocks.EventBus.Abstractions;
using EventDrivenBookingPlatform.Modules.Notifications.Application.Abstractions;
using EventDrivenBookingPlatform.Modules.Reservations.Contracts.IntegrationEvents;

namespace EventDrivenBookingPlatform.Modules.Notifications.Application.EventHandlers;

public class ReservationConfirmedEventHandler : IIntegrationEventHandler<ReservationConfirmedIntegrationEvent>
{
    private readonly IEmailService _emailService;

    public ReservationConfirmedEventHandler(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task Handle(ReservationConfirmedIntegrationEvent @event)
    {
        await _emailService.SendAsync(
            to: $"customer-{@event.CustomerId}@local.test",
            subject: "Reservation Confirmed",
            body: $"Reservation '{@event.ReservationId}' is now confirmed.");
    }
}
