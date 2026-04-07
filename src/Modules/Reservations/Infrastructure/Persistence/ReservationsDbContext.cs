using EventDrivenBookingPlatform.BuildingBlocks.Messaging.Outbox;
using EventDrivenBookingPlatform.BuildingBlocks.SharedKernel;
using EventDrivenBookingPlatform.Modules.Reservations.Contracts.IntegrationEvents;
using EventDrivenBookingPlatform.Modules.Reservations.Domain.Aggregates;
using EventDrivenBookingPlatform.Modules.Reservations.Domain.Events;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json;

namespace EventDrivenBookingPlatform.Modules.Reservations.Infrastructure.Persistence;

public class ReservationsDbContext : DbContext
{
    public ReservationsDbContext(DbContextOptions<ReservationsDbContext> options) : base(options)
    {
    }

    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEntities = ChangeTracker
            .Entries<BaseEntity>()
            .Where(x => x.Entity.DomainEvents.Any())
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());

        var outboxMessages = domainEvents
            .Select(MapToOutboxMessage)
            .Where(x => x is not null)
            .Cast<OutboxMessage>()
            .ToList();

        if (outboxMessages.Count > 0)
        {
            await OutboxMessages.AddRangeAsync(outboxMessages, cancellationToken);
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    private static OutboxMessage? MapToOutboxMessage(DomainEvent domainEvent)
    {
        object? integrationEvent = domainEvent switch
        {
            ReservationCreatedEvent e => new ReservationCreatedIntegrationEvent(
                e.ReservationId,
                e.CustomerId,
                e.ServiceId,
                e.StartDate,
                e.EndDate,
                e.OccurredOn),
            ReservationConfirmedEvent e => new ReservationConfirmedIntegrationEvent(
                e.ReservationId,
                e.CustomerId,
                e.ServiceId,
                e.OccurredOn),
            ReservationCancelledEvent e => new ReservationCancelledIntegrationEvent(
                e.ReservationId,
                e.CustomerId,
                e.ServiceId,
                e.Reason,
                e.OccurredOn),
            _ => null
        };

        if (integrationEvent is null)
        {
            return null;
        }

        return new OutboxMessage
        {
            Id = Guid.NewGuid(),
            OccurredOn = DateTime.UtcNow,
            Type = integrationEvent.GetType().AssemblyQualifiedName ?? integrationEvent.GetType().FullName ?? string.Empty,
            Content = JsonSerializer.Serialize(integrationEvent)
        };
    }
}
