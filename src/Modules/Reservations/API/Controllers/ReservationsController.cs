using EventDrivenBookingPlatform.Modules.Reservations.Application.Commands.CancelReservation;
using EventDrivenBookingPlatform.Modules.Reservations.Application.Commands.ConfirmReservation;
using EventDrivenBookingPlatform.Modules.Reservations.Application.Commands.CreateReservation;
using EventDrivenBookingPlatform.Modules.Reservations.Application.Queries.GetReservationById;
using EventDrivenBookingPlatform.Modules.Reservations.Application.Queries.GetReservationsByCustomer;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventDrivenBookingPlatform.Modules.Reservations.API.Controllers;

[ApiController]
[Route("api/reservations")]
public class ReservationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReservationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateReservation([FromBody] CreateReservationCommand command)
    {
        var reservationId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = reservationId }, reservationId);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var reservation = await _mediator.Send(new GetReservationByIdQuery(id));
        return reservation is null ? NotFound() : Ok(reservation);
    }

    [HttpGet("customer/{customerId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCustomer(Guid customerId)
    {
        var reservations = await _mediator.Send(new GetReservationsByCustomerQuery(customerId));
        return Ok(reservations);
    }

    [HttpPut("{id:guid}/confirm")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Confirm(Guid id)
    {
        await _mediator.Send(new ConfirmReservationCommand(id));
        return NoContent();
    }

    [HttpPut("{id:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelReservationRequest request)
    {
        await _mediator.Send(new CancelReservationCommand(id, request.Reason));
        return NoContent();
    }
}

public record CancelReservationRequest(string Reason);
