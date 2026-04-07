using EventDrivenBookingPlatform.Modules.Audit.Application.Models;

namespace EventDrivenBookingPlatform.Modules.Audit.Application.Interfaces;

public interface IAuditLogStore
{
    Task AddAsync(AuditLog log, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<AuditLog>> GetAllAsync(CancellationToken cancellationToken = default);
}
