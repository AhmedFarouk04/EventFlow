using EventDrivenBookingPlatform.Modules.Users.Application.Interfaces;
using EventDrivenBookingPlatform.Modules.Users.Domain;
using EventDrivenBookingPlatform.Modules.Users.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventDrivenBookingPlatform.Modules.Users.Infrastructure.Repositories;

public class InMemoryUserRepository : IUserRepository
{
    private readonly UsersDbContext _context;

    public InMemoryUserRepository(UsersDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower(), cancellationToken)!;
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken)!;
    }
}
