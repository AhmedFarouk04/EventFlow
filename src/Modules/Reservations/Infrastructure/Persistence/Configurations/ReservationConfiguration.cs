using EventDrivenBookingPlatform.Modules.Reservations.Domain.Aggregates;
using EventDrivenBookingPlatform.Modules.Reservations.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventDrivenBookingPlatform.Modules.Reservations.Infrastructure.Persistence.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("Reservations");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.CustomerId)
            .HasConversion(id => id.Value, value => new CustomerId(value))
            .IsRequired();

        builder.Property(r => r.ServiceId)
            .HasConversion(id => id.Value, value => new ServiceId(value))
            .IsRequired();

        builder.OwnsOne(r => r.DateRange, dateRange =>
        {
            dateRange.Property(d => d.StartDate)
                .HasColumnName("StartDate")
                .IsRequired();

            dateRange.Property(d => d.EndDate)
                .HasColumnName("EndDate")
                .IsRequired();
        });

        builder.Property(r => r.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.HasMany(r => r.Items)
            .WithOne()
            .HasForeignKey("ReservationId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(r => r.DomainEvents);
    }
}
