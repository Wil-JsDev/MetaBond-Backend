using Asp.Versioning;
using MediatR;
using MetaBond.Application.Feature.Events.Commands.Create;
using MetaBond.Application.Feature.Events.Commands.Delete;
using MetaBond.Application.Feature.Events.Commands.Update;
using MetaBond.Application.Feature.Events.Query.FilterByDateRange;
using MetaBond.Application.Feature.Events.Query.FilterByTitle;
using MetaBond.Application.Feature.Events.Query.FilterByTitleCommunityId;
using MetaBond.Application.Feature.Events.Query.GetById;
using MetaBond.Application.Feature.Events.Query.GetCommunitiesAndParticipationInEvent;
using MetaBond.Application.Feature.Events.Query.GetEventsWithParticipationInEvent;
using MetaBond.Application.Feature.Events.Query.GetOrderById;
using MetaBond.Application.Feature.Events.Query.GetParticipationInEvent;
using MetaBond.Application.Feature.Events.Query.Pagination;
using MetaBond.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace MetaBond.Presentation.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:ApiVersion}/events")]
    public class EventsController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateEventsCommand eventsCommand,CancellationToken cancellationToken)
        {
            var result = await mediator.Send(eventsCommand,cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result);
        }

        [HttpDelete("{Id}")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id,CancellationToken cancellationToken)
        {
            var query = new DeleteEventsCommand { Id = id };

            var result = await mediator.Send(query,cancellationToken);
            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpPut("{id}")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateEventsCommand updateEventsCommand,CancellationToken cancellationToken)
        {
            //updateEventsCommand.Id = id;
            var result = await mediator.Send(updateEventsCommand,cancellationToken);
            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        [DisableRateLimiting]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var query = new GetByIdEventsQuery { Id = id };

            var result = await mediator.Send(query, cancellationToken);
            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("communities/{communitiesId}/search/by-date-range")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> FilterByRangeAsync(
            [FromRoute] Guid communitiesId,
            [FromQuery] DateRangeFilter dateRange,
            CancellationToken cancellationToken)
        {
            var query = new FilterByDateRangeEventsQuery
            {
                CommunitiesId = communitiesId,
                DateRangeFilter = dateRange
            };

            var result = await mediator.Send(query,cancellationToken);
            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("search/title/{title}")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> FilterByTitleAsync([FromRoute] string title,CancellationToken cancellationToken)
        {
            var query = new FilterByTitleEventsQuery { Title = title };
            var result = await mediator.Send(query,cancellationToken);
            if (!result.IsSuccess) 
                return NotFound(result.Error);

            return Ok(result.Value);
        }
        
        [HttpGet("communities/{communityId}/search/title/{title}")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetEventsByTitleAndCommunityIdAsync(
            [FromRoute] Guid communityId,
            [FromRoute] string title,
            CancellationToken cancellationToken)
        {
            var query = new GetEventsByTitleAndCommunityIdQuery
            {
                CommunitiesId = communityId,
                Title = title
            };
            var result = await mediator.Send(query,cancellationToken);
            if (!result.IsSuccess) 
                return NotFound(result.Error);

            return Ok(result.Value);
        }
        
        [HttpGet("{eventId}/communities")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetEventsWithCommunitiesAsync([FromRoute] Guid eventId,CancellationToken cancellationToken)
        {
            var query = new GetEventsDetailsQuery { Id = eventId };
            
            var result = await mediator.Send(query,cancellationToken);
            if (!result.IsSuccess)
                return NotFound(result.Error);
            
            return Ok(result.Value);
        }

        [HttpGet("{eventId}/participation-in-event")]
        [DisableRateLimiting]
        public async Task<IActionResult> GetEventsWithParticipationInEventAsync([FromRoute] Guid eventId, CancellationToken cancellationToken)
        {
            var query = new GetEventsWithParticipationInEventQuery { EventsId = eventId };
            var result = await mediator.Send(query,cancellationToken);
            if(!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("order")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> OrderByIdAsync([FromQuery] string orderBy,CancellationToken cancellationToken)
        {
            var query = new GetOrderByIdEventsQuery { Order = orderBy };

            var result = await mediator.Send(query,cancellationToken);
            if(!result.IsSuccess) 
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("{eventId}/participation")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetEventsWithParticipationAsync([FromRoute] Guid eventId,CancellationToken cancellationToken)
        {
            var query = new GetParticipationInEventQuery { EventsId= eventId };

            var result = await mediator.Send(query,cancellationToken);
            if(!result.IsSuccess) 
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("pagination")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetPagedAsync([FromQuery] int pageNumber, [FromQuery] int pageSize,CancellationToken cancellationToken)
        {
            var query = new GetPagedEventsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
            };

            var result = await mediator.Send(query,cancellationToken);
            if (!result.IsSuccess) 
                return NotFound(result.Error);

            return Ok(result.Value);
        }
    }
}
