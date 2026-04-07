using EventDrivenBookingPlatform.Modules.Audit.Application.Interfaces;
using EventDrivenBookingPlatform.Modules.Audit.Application.Models;
using EventDrivenBookingPlatform.Modules.Audit.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventDrivenBookingPlatform.Modules.Audit.Infrastructure.Stores;

public class AuditLogStore : IAuditLogStore
{
    private readonly AuditDbContext _context;

    public AuditLogStore(AuditDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(AuditLog log, CancellationToken cancellationToken = default)
    {
        await _context.AuditLogs.AddAsync(log, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<AuditLog>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .OrderBy(x => x.LoggedAt)
            .ToListAsync(cancellationToken);
    }
}
