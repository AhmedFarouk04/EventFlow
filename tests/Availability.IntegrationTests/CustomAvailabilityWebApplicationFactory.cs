using EventDrivenBookingPlatform.Modules.Availability.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Availability.IntegrationTests;

public class CustomAvailabilityWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"availability-tests-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AvailabilityDbContext>>();
            services.RemoveAll<AvailabilityDbContext>();

            services.AddDbContext<AvailabilityDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));
        });
    }
}
