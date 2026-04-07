using EventDrivenBookingPlatform.Modules.Availability.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace EventDrivenBookingPlatform.Modules.Availability.Infrastructure.Persistence;

public class AvailabilityDbContext : DbContext
{
    public AvailabilityDbContext(DbContextOptions<AvailabilityDbContext> options) : base(options)
    {
    }

    public DbSet<AvailabilitySlot> AvailabilitySlots => Set<AvailabilitySlot>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
