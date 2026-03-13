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

    public async Task<IReadOnlyList<OutboxMessage>> GetUnprocessedMessagesAsync(CancellationToken cancellationToken = default)
    {
        // بنسحب الرسايل اللي لسه متبعتتش وبناخد 20 رسالة بـ 20 رسالة عشان الـ Performance
        return await _dbContext.OutboxMessages
            .Where(m => m.ProcessedOn == null)
            .Take(20)
            .ToListAsync(cancellationToken);
    }

    public async Task MarkAsProcessedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var message = await _dbContext.OutboxMessages.FindAsync(new object[] { id }, cancellationToken);
        if (message != null)
        {
            message.ProcessedOn = DateTime.UtcNow;
        }
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}