using EventDrivenBookingPlatform.Modules.Pricing.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventDrivenBookingPlatform.Modules.Pricing.Infrastructure.Persistence.Configurations;

public class PricingRuleConfiguration : IEntityTypeConfiguration<PricingRule>
{
    public void Configure(EntityTypeBuilder<PricingRule> builder)
    {
        builder.ToTable("PricingRules");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ServiceId).IsRequired();
        builder.Property(x => x.BasePrice).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.SeasonalMultiplier).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.ServiceFee).HasColumnType("decimal(18,2)").IsRequired();
        builder.HasIndex(x => x.ServiceId).IsUnique();
    }
}
