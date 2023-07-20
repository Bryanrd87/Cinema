using Application.Features.Showtime;
using Application.Features.Showtime.Commands;
using Application.Features.Showtime.Queries.GetShowtimeDetails;
using Application.Features.Showtime.Queries.GetShowtimes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ApiApplication.Controllers
{
    [Route("api/[controller]")]    
    [ApiController]
    public class ShowtimeController : ControllerBase
    {       
        private readonly IMediator _mediator;
        public ShowtimeController(IMediator mediator)
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
        public async Task<ActionResult<List<ShowtimeDTO>>> Get()
        {
            var showtimes = await _mediator.Send(new GetShowtimeListQuery());
            return Ok(showtimes);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ShowtimeDetailsDTO>> Get(int id)
        {
            var showtime = await _mediator.Send(new GetShowtimeDetailsQuery { Id = id });
            return Ok(showtime);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]        
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Post(CreateShowtimeCommand createShowtime, CancellationToken cancellationToken)
        {            
            var response = await _mediator.Send(createShowtime);
            return CreatedAtAction(nameof(Get), new { id = response });
        }
    }
}
