using EventDrivenBookingPlatform.Modules.Audit.Application.Interfaces;
using EventDrivenBookingPlatform.Modules.Audit.Application.Models;

namespace EventDrivenBookingPlatform.Modules.Audit.Infrastructure.Stores;

public class InMemoryAuditLogStore : IAuditLogStore
{
    private readonly List<AuditLog> _logs = [];

    public Task AddAsync(AuditLog log, CancellationToken cancellationToken = default)
    {
        _logs.Add(log);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<AuditLog>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult((IReadOnlyCollection<AuditLog>)_logs.ToList());
    }
}
