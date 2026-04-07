using System.Text.Json;
using EventDrivenBookingPlatform.BuildingBlocks.EventBus.Abstractions;
using EventDrivenBookingPlatform.Modules.Audit.Application.Interfaces;
using EventDrivenBookingPlatform.Modules.Audit.Application.Models;
using EventDrivenBookingPlatform.Modules.Availability.Contracts.IntegrationEvents;
using EventDrivenBookingPlatform.Modules.Pricing.Contracts.IntegrationEvents;
using EventDrivenBookingPlatform.Modules.Reservations.Contracts.IntegrationEvents;

namespace EventDrivenBookingPlatform.Modules.Audit.Application.EventHandlers;

public class GenericAuditEventHandler<TIntegrationEvent> : IIntegrationEventHandler<TIntegrationEvent>
    where TIntegrationEvent : IIntegrationEvent
{
    private readonly IAuditLogStore _store;

    public GenericAuditEventHandler(IAuditLogStore store)
    {
        _store = store;
    }

    public async Task Handle(TIntegrationEvent @event)
    {
        await _store.AddAsync(new AuditLog
        {
            Id = Guid.NewGuid(),
            EventName = typeof(TIntegrationEvent).Name,
            Payload = JsonSerializer.Serialize(@event),
            LoggedAt = DateTime.UtcNow
        });
    }
}

public sealed class ReservationCreatedAuditHandler : GenericAuditEventHandler<ReservationCreatedIntegrationEvent>
{
    public ReservationCreatedAuditHandler(IAuditLogStore store) : base(store) { }
}

public sealed class ReservationConfirmedAuditHandler : GenericAuditEventHandler<ReservationConfirmedIntegrationEvent>
{
    public ReservationConfirmedAuditHandler(IAuditLogStore store) : base(store) { }
}

public sealed class ReservationCancelledAuditHandler : GenericAuditEventHandler<ReservationCancelledIntegrationEvent>
{
    public ReservationCancelledAuditHandler(IAuditLogStore store) : base(store) { }
}

public sealed class AvailabilityBlockedAuditHandler : GenericAuditEventHandler<AvailabilityBlockedIntegrationEvent>
{
    public AvailabilityBlockedAuditHandler(IAuditLogStore store) : base(store) { }
}

public sealed class PriceCalculatedAuditHandler : GenericAuditEventHandler<PriceCalculatedIntegrationEvent>
{
    public PriceCalculatedAuditHandler(IAuditLogStore store) : base(store) { }
}
