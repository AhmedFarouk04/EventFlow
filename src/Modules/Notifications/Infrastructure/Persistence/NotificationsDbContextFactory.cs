using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EventDrivenBookingPlatform.Modules.Notifications.Infrastructure.Persistence;

public class NotificationsDbContextFactory : IDesignTimeDbContextFactory<NotificationsDbContext>
{
    public NotificationsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<NotificationsDbContext>();
        optionsBuilder.UseSqlServer(
            "Server=(localdb)\\mssqllocaldb;Database=EventDrivenBookingNotificationsDb;Trusted_Connection=True;MultipleActiveResultSets=true");

        return new NotificationsDbContext(optionsBuilder.Options);
    }
}
