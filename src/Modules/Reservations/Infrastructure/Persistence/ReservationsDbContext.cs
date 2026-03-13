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

    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; } // ضفنا جدول الأوت بوكس

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    // السحر كله بيحصل هنا قبل ما نسيف في الداتابيز
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // 1. نجيب كل الكيانات اللي حصل فيها أحداث
        var domainEntities = ChangeTracker
            .Entries<BaseEntity>()
            .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any())
            .ToList();

        // 2. نسحب الأحداث دي
        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        // 3. نفضي الأحداث من الكيانات عشان متتنفذش تاني
        domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());

        // 4. نحول الـ Domain Event لـ Integration Event ونجهزه يتحفظ في الـ Outbox
        var outboxMessages = domainEvents.Select(domainEvent =>
        {
            object? integrationEvent = domainEvent switch
            {
                ReservationCreatedDomainEvent e => new ReservationCreatedIntegrationEvent(
                    e.ReservationId,
                    e.CustomerEmail,
                    e.CheckInDate,
                    e.CheckOutDate,
                    e.TotalPrice),
                _ => null
            };

            if (integrationEvent == null) return null;

            return new OutboxMessage
            {
                Id = Guid.NewGuid(),
                OccurredOn = DateTime.UtcNow,
                Type = integrationEvent.GetType().AssemblyQualifiedName ?? integrationEvent.GetType().FullName!,
                Content = JsonSerializer.Serialize(integrationEvent)
            };
        }).Where(m => m != null).ToList();

        // 5. نضيف الرسايل دي لجدول الـ Outbox
        if (outboxMessages.Any())
        {
            await OutboxMessages.AddRangeAsync(outboxMessages!, cancellationToken);
        }

        // 6. نحفظ كل حاجة (الحجز + الرسالة) في خطوة واحدة جوه نفس الـ Transaction
        return await base.SaveChangesAsync(cancellationToken);
    }
}