using NetArchTest.Rules;

namespace Architecture.Tests;

public class ArchitectureTests
{
    [Fact]
    public void Domain_ShouldNotDependOnInfrastructure()
    {
        var result = Types.InAssembly(typeof(EventDrivenBookingPlatform.Modules.Reservations.Domain.Aggregates.Reservation).Assembly)
            .ShouldNot()
            .HaveDependencyOn("EventDrivenBookingPlatform.Modules.Reservations.Infrastructure")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Controllers_ShouldNotDependOnInfrastructureNamespaceDirectly()
    {
        var result = Types.InAssembly(typeof(EventDrivenBookingPlatform.Modules.Reservations.API.Controllers.ReservationsController).Assembly)
            .That()
            .ResideInNamespace("EventDrivenBookingPlatform.Modules.Reservations.API.Controllers")
            .ShouldNot()
            .HaveDependencyOn("EventDrivenBookingPlatform.Modules.Reservations.Infrastructure")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Application_ShouldNotDependOnApi()
    {
        var result = Types.InAssembly(typeof(EventDrivenBookingPlatform.Modules.Reservations.Application.Commands.CreateReservation.CreateReservationCommand).Assembly)
            .ShouldNot()
            .HaveDependencyOn("EventDrivenBookingPlatform.Modules.Reservations.API")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void AvailabilityDomain_ShouldNotDependOnInfrastructure()
    {
        var result = Types.InAssembly(typeof(EventDrivenBookingPlatform.Modules.Availability.Domain.Aggregates.AvailabilitySlot).Assembly)
            .ShouldNot()
            .HaveDependencyOn("EventDrivenBookingPlatform.Modules.Availability.Infrastructure")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void AvailabilityControllers_ShouldNotDependOnInfrastructureNamespaceDirectly()
    {
        var result = Types.InAssembly(typeof(EventDrivenBookingPlatform.Modules.Availability.API.Controllers.AvailabilityController).Assembly)
            .That()
            .ResideInNamespace("EventDrivenBookingPlatform.Modules.Availability.API.Controllers")
            .ShouldNot()
            .HaveDependencyOn("EventDrivenBookingPlatform.Modules.Availability.Infrastructure")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }
}
