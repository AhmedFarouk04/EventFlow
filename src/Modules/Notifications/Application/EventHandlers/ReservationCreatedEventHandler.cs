using EventDrivenBookingPlatform.BuildingBlocks.EventBus.Abstractions;
using EventDrivenBookingPlatform.Modules.Notifications.Application.Abstractions;
using EventDrivenBookingPlatform.Modules.Reservations.Contracts.IntegrationEvents;

namespace EventDrivenBookingPlatform.Modules.Notifications.Application.EventHandlers;

public class ReservationCreatedEventHandler : IIntegrationEventHandler<ReservationCreatedIntegrationEvent>
{
    private readonly IEmailService _emailService;

    public ReservationCreatedEventHandler(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task Handle(ReservationCreatedIntegrationEvent @event)
    {
        await _emailService.SendAsync(
            to: $"customer-{@event.CustomerId}@local.test",
            subject: "Reservation Created",
            body: $"Reservation '{@event.ReservationId}' has been created from {@event.StartDate:d} to {@event.EndDate:d}.");
    }
}
