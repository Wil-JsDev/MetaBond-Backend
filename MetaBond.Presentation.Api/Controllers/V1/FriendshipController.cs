using Asp.Versioning;
using MediatR;
using MetaBond.Application.Feature.Friendship.Command.Create;
using MetaBond.Application.Feature.Friendship.Command.Delete;
using MetaBond.Application.Feature.Friendship.Command.Update;
using MetaBond.Application.Feature.Friendship.Query.GetById;
using MetaBond.Application.Feature.Friendship.Query.GetCountByStatus;
using MetaBond.Application.Feature.Friendship.Query.GetCreated.GetCreatedAfter;
using MetaBond.Application.Feature.Friendship.Query.GetCreated.GetCreatedBefore;
using MetaBond.Application.Feature.Friendship.Query.GetFilterByStatus;
using MetaBond.Application.Feature.Friendship.Query.GetOrderById;
using MetaBond.Application.Feature.Friendship.Query.GetRecentlyCreated;
using MetaBond.Application.Feature.Friendship.Query.Pagination;
using MetaBond.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace MetaBond.Presentation.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/{version:ApiVersion}/friendship")]
    public class FriendshipController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FriendshipController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateFriendshipCommand friendshipCommand,CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(friendshipCommand,cancellationToken);
            if(!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result);
        }

        [HttpGet("{id}")]
        [DisableRateLimiting]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id,CancellationToken cancellationToken)
        {
            var query = new GetByIdFriendshipQuery { Id = id };

            var result = await _mediator.Send(query,cancellationToken);
            if(!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpDelete("{id}")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id,CancellationToken cancellationToken)
        {
            var query = new DeleteFriendshipCommand {Id = id};

            var result = await _mediator.Send(query,cancellationToken);
            if(!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result);
        }

        [HttpPut("{id}")]
        [DisableRateLimiting]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateFriendshipCommand updateFriendshipCommand,CancellationToken cancellationToken)
        {
            updateFriendshipCommand.Id = id;

            var result = await _mediator.Send(updateFriendshipCommand,cancellationToken);
            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("count/status/{status}")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> FilterByStatus([FromRoute] Status status,CancellationToken cancellationToken)
        {
            var query = new GetCountByStatusFriendshipQuery {Status = status};

            var result = await _mediator.Send(query, cancellationToken);
            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }


        [HttpGet("after-created")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetAfterCreatedAsync([FromQuery] DateRangeType dateRange,CancellationToken cancellationToken)
        {
            var query = new GetCreatedAfterFriendshipQuery {DateRange = dateRange};
            
            var result = await _mediator.Send(query,cancellationToken);
            if (!result.IsSuccess) 
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("before-created")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetBeforeCreatedAsync([FromQuery] PastDateRangeType pastDateRange,CancellationToken cancellationToken )
        {
            var query = new GetCreatedBeforeFriendshipQuery { PastDateRangeType = pastDateRange};

            var result = await _mediator.Send(query, cancellationToken);
            if(!result.IsSuccess) 
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("filter-status")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetFilterByStatus([FromQuery] Status status,CancellationToken cancellationToken)
        {
            var query = new FilterByStatusFriendshipQuery { Status = status};

            var result = await _mediator.Send(query,cancellationToken);
            if(!result.IsSuccess) 
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("order-by-id")]
        [DisableRateLimiting]
        public async Task<IActionResult> OrderbyIdAscOrDescAsync([FromQuery] string sort,CancellationToken cancellationToken)
        {
            var query = new GetOrderByIdFriendshipQuery { Sort = sort };

            var result = await _mediator.Send(query, cancellationToken);
            if(!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("recent-created/{limit}")]
        [DisableRateLimiting]
        public async Task<IActionResult> GetRecentCreatedAsync([FromRoute] int limit,CancellationToken cancellationToken)
        {
            var query = new GetRecentlyCreatedFriendshipQuery { Limit = limit };

            var result = await _mediator.Send(query, cancellationToken);
            if(!result.IsSuccess)
                return BadRequest(result.Error);

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
