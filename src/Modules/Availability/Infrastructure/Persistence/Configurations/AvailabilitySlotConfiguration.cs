using EventDrivenBookingPlatform.Modules.Availability.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventDrivenBookingPlatform.Modules.Availability.Infrastructure.Persistence.Configurations;

public class AvailabilitySlotConfiguration : IEntityTypeConfiguration<AvailabilitySlot>
{
    public void Configure(EntityTypeBuilder<AvailabilitySlot> builder)
    {
        builder.ToTable("AvailabilitySlots");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ServiceId)
            .IsRequired();

        builder.Property(x => x.Date)
            .IsRequired();

        builder.Property(x => x.Capacity)
            .IsRequired();

        builder.Property(x => x.Reserved)
            .IsRequired();

        builder.Property(x => x.Version)
            .IsConcurrencyToken();

        builder.Ignore(x => x.Remaining);
        builder.Ignore(x => x.DomainEvents);

        builder.HasIndex(x => new { x.ServiceId, x.Date })
            .IsUnique();
    }
}
