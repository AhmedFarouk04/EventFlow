using System.Net;
using System.Net.Http.Json;
using EventDrivenBookingPlatform.Modules.Availability.Application.Queries.GetAvailableSlots;

namespace Availability.IntegrationTests;

public class AvailabilityControllerTests : IClassFixture<CustomAvailabilityWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AvailabilityControllerTests(CustomAvailabilityWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task BlockThenCheck_ShouldReturnUnavailableForRequestedQuantity()
    {
        var serviceId = Guid.NewGuid();
        var date = DateTime.UtcNow.Date.AddDays(3);

        var blockResponse = await _client.PostAsJsonAsync("/api/availability/block", new
        {
            serviceId,
            date,
            quantity = 4
        });

        Assert.Equal(HttpStatusCode.NoContent, blockResponse.StatusCode);

        var checkResponse = await _client.GetAsync($"/api/availability/check?serviceId={serviceId}&date={date:O}&quantity=7");
        Assert.Equal(HttpStatusCode.OK, checkResponse.StatusCode);

        var payload = await checkResponse.Content.ReadAsStringAsync();
        Assert.Contains("\"available\":false", payload, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task BlockThenRelease_ShouldRestoreRemainingCapacity()
    {
        var serviceId = Guid.NewGuid();
        var date = DateTime.UtcNow.Date.AddDays(4);

        await _client.PostAsJsonAsync("/api/availability/block", new
        {
            serviceId,
            date,
            quantity = 3
        });

        var releaseResponse = await _client.PostAsJsonAsync("/api/availability/release", new
        {
            serviceId,
            date,
            quantity = 2
        });

        Assert.Equal(HttpStatusCode.NoContent, releaseResponse.StatusCode);

        var slots = await _client.GetFromJsonAsync<List<AvailableSlotDto>>($"/api/availability/slots?serviceId={serviceId}&fromDate={date:O}&toDate={date:O}");

        Assert.NotNull(slots);
        var slot = Assert.Single(slots!);
        Assert.Equal(9, slot.Remaining);
    }
}
