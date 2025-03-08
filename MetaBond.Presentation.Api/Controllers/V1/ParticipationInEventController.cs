using Asp.Versioning;
using MediatR;
using MetaBond.Application.Feature.Friendship.Command.Create;
using MetaBond.Application.Feature.Friendship.Command.Update;
using MetaBond.Application.Feature.Friendship.Query.GetById;
using MetaBond.Application.Feature.Friendship.Query.Pagination;
using MetaBond.Application.Feature.ParticipationInEvent.Querys.GetEvents;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace MetaBond.Presentation.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:ApiVersion}/participation-in-event")]
    public class ParticipationInEventController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ParticipationInEventController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateFriendshipCommand createFriendshipCommand,CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(createFriendshipCommand,cancellationToken);
            if(!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result);
        }

        [HttpPut("{id}")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateFriendshipCommand updateFriendship,CancellationToken cancellationToken)
        {
            updateFriendship.Id = id;

            var result = await _mediator.Send(updateFriendship,cancellationToken);
            if (!result.IsSuccess) 
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        [DisableRateLimiting]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id,CancellationToken cancellationToken)
        {
            var query = new GetByIdFriendshipQuery {Id = id};

            var result = await _mediator.Send(query,cancellationToken);
            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("{eventId}/participations")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetParticipationInEventDetailsAsync([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var query = new GetEventsQuery { EventsId = id};

            var result = await _mediator.Send(query, cancellationToken);
            if(!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("pagination")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetPagedAsync([FromQuery] int pageNumber, [FromQuery] int pageSize,CancellationToken cancellationToken)
        {
            var query = new GetPagedFriendshipQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query,cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }
    }
}
