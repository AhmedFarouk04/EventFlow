using EventDrivenBookingPlatform.Modules.Reservations.Domain.Events;

namespace EventDrivenBookingPlatform.Modules.Reservations.Application.Events;

public class ReservationCreatedDomainEventHandler
{
    public Task HandleAsync(ReservationCreatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
