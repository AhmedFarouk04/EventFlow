using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventDrivenBookingPlatform.Modules.Pricing.Infrastructure.Persistence.Configurations;

public class PriceCalculationRecordConfiguration : IEntityTypeConfiguration<PriceCalculationRecord>
{
    public void Configure(EntityTypeBuilder<PriceCalculationRecord> builder)
    {
        builder.ToTable("PriceCalculations");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.BasePrice).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.TotalPrice).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.NumberOfDays).IsRequired();
        builder.Property(x => x.CalculatedAt).IsRequired();
        builder.HasIndex(x => x.ReservationId);
    }
}
