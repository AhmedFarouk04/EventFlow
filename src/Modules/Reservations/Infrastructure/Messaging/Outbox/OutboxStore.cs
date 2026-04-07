using EventDrivenBookingPlatform.BuildingBlocks.Messaging.Outbox;
using EventDrivenBookingPlatform.Modules.Reservations.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventDrivenBookingPlatform.Modules.Reservations.Infrastructure.Messaging.Outbox;

public class OutboxStore : IOutboxStore
{
    private readonly ReservationsDbContext _dbContext;

    public OutboxStore(ReservationsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        await _dbContext.OutboxMessages.AddAsync(message, cancellationToken);
    }

    public async Task<IReadOnlyList<OutboxMessage>> GetUnprocessedMessagesAsync(int batchSize, CancellationToken cancellationToken = default)
    {
        return await _dbContext.OutboxMessages
            .Where(m => m.ProcessedOn == null)
            .OrderBy(m => m.OccurredOn)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public async Task MarkAsProcessedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var message = await _dbContext.OutboxMessages.FindAsync([id], cancellationToken);
        if (message is null)
        {
            return;
        }

        message.ProcessedOn = DateTime.UtcNow;
        message.Error = null;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkAsFailedAsync(Guid id, string error, CancellationToken cancellationToken = default)
    {
        var message = await _dbContext.OutboxMessages.FindAsync([id], cancellationToken);
        if (message is null)
        {
            return;
        }

        message.RetryCount += 1;
        message.LastRetryOn = DateTime.UtcNow;
        message.Error = error;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
