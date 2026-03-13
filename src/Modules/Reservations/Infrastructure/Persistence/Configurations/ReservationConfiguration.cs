using EventDrivenBookingPlatform.Modules.Reservations.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventDrivenBookingPlatform.Modules.Reservations.Infrastructure.Persistence.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("Reservations");
        builder.HasKey(r => r.Id);

        // Configure Value Objects (Owned Types)
        builder.OwnsOne(r => r.CustomerInfo, ci =>
        {
            ci.Property(c => c.FullName).HasMaxLength(100).IsRequired();
            ci.Property(c => c.Email).HasMaxLength(255).IsRequired();
            ci.Property(c => c.PhoneNumber).HasMaxLength(15).IsRequired();
            ci.Property(c => c.Nationality).HasMaxLength(2).IsRequired();
            ci.Property(c => c.PassportNumber).HasMaxLength(12);
        });

        builder.OwnsOne(r => r.TripDetails, td =>
        {
            td.Property(t => t.TripId).IsRequired();
            td.Property(t => t.TripName).HasMaxLength(200).IsRequired();
            td.Property(t => t.Destination).HasMaxLength(100).IsRequired();
            td.Property(t => t.Duration).IsRequired();
        });

        builder.OwnsOne(r => r.PriceDetails, pd =>
        {
            pd.Property(p => p.BasePrice).HasColumnType("decimal(18,2)").IsRequired();
            pd.Property(p => p.Taxes).HasColumnType("decimal(18,2)").IsRequired();
            pd.Property(p => p.Discounts).HasColumnType("decimal(18,2)").IsRequired();
            pd.Property(p => p.TotalPrice).HasColumnType("decimal(18,2)").IsRequired();
            pd.Property(p => p.Currency).HasMaxLength(3).IsRequired();
        });

        builder.Property(r => r.SpecialRequests).HasMaxLength(500);

        // Optimistic Concurrency
        builder.Property(r => r.Version).IsConcurrencyToken();

        // Ignore Domain Events so EF doesn't try to map them to a database column
        builder.Ignore(r => r.DomainEvents);
    }
}