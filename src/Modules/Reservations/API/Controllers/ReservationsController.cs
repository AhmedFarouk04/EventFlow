using EventDrivenBookingPlatform.Modules.Reservations.Application.Commands.CreateReservation;
using EventDrivenBookingPlatform.Modules.Reservations.Application.Queries.GetReservationById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace EventDrivenBookingPlatform.Modules.Reservations.API.Controllers;

[ApiController]
[Route("api/modules/reservations")]
public class ReservationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReservationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateReservation([FromBody] CreateReservationCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new { Error = result.Error });
        }

        return CreatedAtAction(nameof(GetReservation), new { id = result.Value }, result.Value);
    }

    

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetReservation(Guid id)
    {
        var query = new GetReservationByIdQuery(id);
        var reservation = await _mediator.Send(query);

        if (reservation == null)
        {
            return NotFound(new { message = $"Reservation with ID {id} not found." });
        }

        return Ok(reservation);
    }
}