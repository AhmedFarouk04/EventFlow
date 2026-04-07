using EventDrivenBookingPlatform.BuildingBlocks.EventBus.Abstractions;
using EventDrivenBookingPlatform.Modules.Audit.Application.EventHandlers;
using EventDrivenBookingPlatform.Modules.Reservations.Contracts.IntegrationEvents;
using EventDrivenBookingPlatform.Modules.Availability.Contracts.IntegrationEvents;
using EventDrivenBookingPlatform.Modules.Pricing.Contracts.IntegrationEvents;
using Microsoft.Extensions.DependencyInjection;

namespace EventDrivenBookingPlatform.Modules.Audit.Application.DependencyInjection;

public static class AuditModuleExtensions
{
    public static IServiceCollection AddAuditApplication(this IServiceCollection services)
    {
        services.AddScoped<ReservationCreatedAuditHandler>();
        services.AddScoped<ReservationConfirmedAuditHandler>();
        services.AddScoped<ReservationCancelledAuditHandler>();
        services.AddScoped<AvailabilityBlockedAuditHandler>();
        services.AddScoped<PriceCalculatedAuditHandler>();
        return services;
    }

    public static void AddAuditSubscriptions(this IEventBus eventBus)
    {
        eventBus.Subscribe<ReservationCreatedIntegrationEvent, ReservationCreatedAuditHandler>();
        eventBus.Subscribe<ReservationConfirmedIntegrationEvent, ReservationConfirmedAuditHandler>();
        eventBus.Subscribe<ReservationCancelledIntegrationEvent, ReservationCancelledAuditHandler>();
        eventBus.Subscribe<AvailabilityBlockedIntegrationEvent, AvailabilityBlockedAuditHandler>();
        eventBus.Subscribe<PriceCalculatedIntegrationEvent, PriceCalculatedAuditHandler>();
    }
}
