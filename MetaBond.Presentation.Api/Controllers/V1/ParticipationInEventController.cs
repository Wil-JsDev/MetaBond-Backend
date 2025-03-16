using Asp.Versioning;
using MediatR;
using MetaBond.Application.Feature.Friendship.Command.Create;
using MetaBond.Application.Feature.Friendship.Command.Update;
using MetaBond.Application.Feature.Friendship.Query.GetById;
using MetaBond.Application.Feature.Friendship.Query.Pagination;
using MetaBond.Application.Feature.ParticipationInEvent.Commands.Create;
using MetaBond.Application.Feature.ParticipationInEvent.Commands.Update;
using MetaBond.Application.Feature.ParticipationInEvent.Querys.GetById;
using MetaBond.Application.Feature.ParticipationInEvent.Querys.GetEvents;
using MetaBond.Application.Feature.ParticipationInEvent.Querys.Pagination;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace MetaBond.Presentation.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:ApiVersion}/participation-in-event")]
    public class ParticipationInEventController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateParticipationInEventCommand createParticipation,CancellationToken cancellationToken)
        {
            var result = await mediator.Send(createParticipation,cancellationToken);
            if(!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result);
        }

        [HttpPut("{id}")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateParticipationInEventCommand updateParticipationInEventCommand,CancellationToken cancellationToken)
        {
            var result = await mediator.Send(updateParticipationInEventCommand,cancellationToken);
            if (!result.IsSuccess) 
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        [DisableRateLimiting]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id,CancellationToken cancellationToken)
        {
            var query = new GetByIdParticipationInEventQuery {ParticipationInEventId = id};

            var result = await mediator.Send(query,cancellationToken);
            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("{eventId}/participation")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetParticipationInEventDetailsAsync([FromRoute] Guid eventId, CancellationToken cancellationToken)
        {
            var query = new GetEventsQuery { EventsId = eventId};

            var result = await mediator.Send(query, cancellationToken);
            if(!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("pagination")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetPagedAsync([FromQuery] int pageNumber, [FromQuery] int pageSize,CancellationToken cancellationToken)
        {
            var query = new GetPagedParticipationInEventQuery()
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await mediator.Send(query,cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }
    }
}
