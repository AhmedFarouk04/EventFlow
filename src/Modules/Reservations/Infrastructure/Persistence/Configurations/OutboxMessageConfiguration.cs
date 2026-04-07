using EventDrivenBookingPlatform.BuildingBlocks.Messaging.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventDrivenBookingPlatform.Modules.Reservations.Infrastructure.Persistence.Configurations;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Content).IsRequired();
        builder.Property(x => x.OccurredOn).IsRequired();
        builder.Property(x => x.ProcessedOn);
        builder.Property(x => x.RetryCount).IsRequired();
        builder.Property(x => x.LastRetryOn);
        builder.Property(x => x.Error).HasMaxLength(2000);

        builder.HasIndex(x => x.ProcessedOn);
    }
}
