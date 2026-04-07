using EventDrivenBookingPlatform.Modules.Reservations.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventDrivenBookingPlatform.Modules.Reservations.Infrastructure.Persistence.Configurations;

public class ReservationItemConfiguration : IEntityTypeConfiguration<ReservationItem>
{
    public void Configure(EntityTypeBuilder<ReservationItem> builder)
    {
        builder.ToTable("ReservationItems");
        builder.HasKey(i => i.Id);

        builder.Property<Guid>("ReservationId").IsRequired();
        builder.Property(i => i.Name).HasMaxLength(150).IsRequired();
        builder.Property(i => i.Quantity).IsRequired();
        builder.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();

        builder.Ignore(i => i.DomainEvents);
    }
}
