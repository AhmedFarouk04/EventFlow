using EventDrivenBookingPlatform.Modules.Reservations.Application.Commands.CreateReservation;
using EventDrivenBookingPlatform.Modules.Reservations.Application.Interfaces;
using EventDrivenBookingPlatform.Modules.Reservations.Domain.Aggregates;
using EventDrivenBookingPlatform.Modules.Reservations.Domain.ValueObjects;

namespace Reservations.UnitTests.Application;

public class CreateReservationHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCreateReservation_WhenNoOverlapExists()
    {
        var repository = new InMemoryReservationRepository();
        var handler = new CreateReservationHandler(repository);

        var reservationId = await handler.Handle(
            new CreateReservationCommand(
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTime.UtcNow.Date.AddDays(1),
                DateTime.UtcNow.Date.AddDays(2),
                [new CreateReservationItemRequest("Room", 1, 100)]),
            CancellationToken.None);

        Assert.NotEqual(Guid.Empty, reservationId);
        Assert.Single(repository.StoredReservations);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenOverlapExists()
    {
        var repository = new InMemoryReservationRepository(hasOverlap: true);
        var handler = new CreateReservationHandler(repository);

        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(
            new CreateReservationCommand(
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTime.UtcNow.Date.AddDays(1),
                DateTime.UtcNow.Date.AddDays(2),
                null),
            CancellationToken.None));
    }

    private sealed class InMemoryReservationRepository : IReservationRepository
    {
        private readonly bool _hasOverlap;

        public InMemoryReservationRepository(bool hasOverlap = false)
        {
            _hasOverlap = hasOverlap;
        }

        public List<Reservation> StoredReservations { get; } = new();

        public Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default)
        {
            StoredReservations.Add(reservation);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<Reservation>> GetByCustomerIdAsync(CustomerId customerId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult((IReadOnlyCollection<Reservation>)StoredReservations.Where(r => r.CustomerId == customerId).ToList());
        }

        public Task<Reservation?> GetByIdAsync(ReservationId id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(StoredReservations.FirstOrDefault(r => r.Id == id.Value));
        }

        public Task<bool> HasOverlapAsync(CustomerId customerId, DateRange dateRange, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_hasOverlap);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Reservation reservation, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
