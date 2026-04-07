using System.Net;
using System.Net.Http.Json;

namespace Reservations.IntegrationTests;

public class ReservationsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public ReservationsControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _factory.EnsureSubscriptions();
    }

    [Fact]
    public async Task CreateReservation_ThenGetById_ShouldReturnCreatedReservation()
    {
        var request = new
        {
            customerId = Guid.NewGuid(),
            serviceId = Guid.NewGuid(),
            startDate = DateTime.UtcNow.Date.AddDays(2),
            endDate = DateTime.UtcNow.Date.AddDays(4),
            items = new[]
            {
                new { name = "Room", quantity = 1, unitPrice = 150m }
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/reservations", request);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var reservationId = await createResponse.Content.ReadFromJsonAsync<Guid>();
        var getResponse = await _client.GetAsync($"/api/reservations/{reservationId}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var payload = await getResponse.Content.ReadAsStringAsync();
        Assert.Contains(reservationId.ToString(), payload);
    }

    [Fact]
    public async Task CreateReservation_ThenProcessOutbox_ShouldTriggerModuleSideEffects()
    {
        var serviceId = Guid.NewGuid();
        var startDate = DateTime.UtcNow.Date.AddDays(5);
        var request = new
        {
            customerId = Guid.NewGuid(),
            serviceId,
            startDate,
            endDate = startDate.AddDays(2),
            items = new[]
            {
                new { name = "Room", quantity = 1, unitPrice = 150m }
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/reservations", request);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var reservationId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        await _factory.ProcessOutboxAsync();

        var availabilityRepository = _factory.GetAvailabilityRepository();
        var firstDaySlot = await availabilityRepository.FindAsync(serviceId, startDate);
        var secondDaySlot = await availabilityRepository.FindAsync(serviceId, startDate.AddDays(1));

        Assert.NotNull(firstDaySlot);
        Assert.NotNull(secondDaySlot);
        Assert.Equal(9, firstDaySlot!.Remaining);
        Assert.Equal(9, secondDaySlot!.Remaining);

        var auditLogStore = _factory.GetAuditLogStore();
        var auditLogs = (await auditLogStore.GetAllAsync())
            .Where(x => x.Payload.Contains(reservationId.ToString(), StringComparison.OrdinalIgnoreCase))
            .ToList();
        Assert.Contains(auditLogs, x => x.EventName == "ReservationCreatedIntegrationEvent");
        Assert.Contains(auditLogs, x => x.EventName == "AvailabilityBlockedIntegrationEvent");
        Assert.Contains(auditLogs, x => x.EventName == "PriceCalculatedIntegrationEvent");
        Assert.Equal(2, auditLogs.Count(x => x.EventName == "AvailabilityBlockedIntegrationEvent"));

        var emailService = _factory.GetEmailService();
        Assert.Contains(emailService.SentEmails, x => x.Subject == "Reservation Created");
    }
}
