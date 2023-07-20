using Application.Features.Ticket;
using Application.Features.Ticket.Commands.Buy;
using Application.Features.Ticket.Commands.Reserve;
using Application.Features.Ticket.Queries.GetTicketDetails;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Features.Ticket.Queries.GetTickets;

namespace ApiApplication.Controllers
{
    [Route("api/[controller]")]   
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TicketController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<List<ReservedTicketDTO>>> Get(CancellationToken cancellationToken)
        {
            var tickets = await _mediator.Send(new GetTicketsListQuery(), cancellationToken);
            return Ok(tickets);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]       
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReservedTicketDTO>> Get(Guid ticketId, CancellationToken cancellationToken)
        {
            var ticket = await _mediator.Send(new GetTicketDetailQuery { Id = ticketId }, cancellationToken);
            return Ok(ticket);
        }

        [HttpPost("reserve")]
        [Authorize(Roles = "Admin")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]       
        public async Task<ActionResult> ReserveSeats(ReserveSeatsCommand reserveSeatsCommands, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(reserveSeatsCommands, cancellationToken);
            return Ok(response);
        }

        [HttpPut("buy")]
        [Authorize(Roles = "Admin")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]        
        public async Task<ActionResult> BuySeats(Guid ticketId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new BuySeatsCommand { Id = ticketId }, cancellationToken);
            return Ok(response);
        }
    }
}
