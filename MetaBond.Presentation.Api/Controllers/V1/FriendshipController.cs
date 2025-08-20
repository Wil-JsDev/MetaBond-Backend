using Asp.Versioning;
using MediatR;
using MetaBond.Application.Feature.Friendship.Commands.Create;
using MetaBond.Application.Feature.Friendship.Commands.Delete;
using MetaBond.Application.Feature.Friendship.Commands.Update;
using MetaBond.Application.Feature.Friendship.Query.GetById;
using MetaBond.Application.Feature.Friendship.Query.GetCountByStatus;
using MetaBond.Application.Feature.Friendship.Query.GetCreated.GetCreatedAfter;
using MetaBond.Application.Feature.Friendship.Query.GetCreated.GetCreatedBefore;
using MetaBond.Application.Feature.Friendship.Query.GetFilterByStatus;
using MetaBond.Application.Feature.Friendship.Query.GetFriendshipWithUser;
using MetaBond.Application.Feature.Friendship.Query.GetOrderById;
using MetaBond.Application.Feature.Friendship.Query.GetRecentlyCreated;
using MetaBond.Application.Feature.Friendship.Query.Pagination;
using MetaBond.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

namespace MetaBond.Presentation.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/{version:ApiVersion}/friendship")]
public class FriendshipController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Create a new friendship",
        Description = "Creates a new friendship using the provided command data."
    )]
    public async Task<IActionResult> CreateAsync([FromBody] CreateFriendshipCommand friendshipCommand,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(friendshipCommand, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result);
    }

    [HttpGet("{id}")]
    [DisableRateLimiting]
    [SwaggerOperation(
        Summary = "Get friendship by ID",
        Description = "Retrieves a specific friendship by its unique identifier."
    )]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetByIdFriendshipQuery { Id = id };
        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpDelete("{id}")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Delete a friendship by ID",
        Description = "Deletes the friendship identified by the given ID."
    )]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new DeleteFriendshipCommand { Id = id };
        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result);
    }

    [HttpPut]
    [DisableRateLimiting]
    [SwaggerOperation(
        Summary = "Update a friendship",
        Description = "Updates an existing friendship using the provided command data."
    )]
    public async Task<IActionResult> UpdateAsync([FromBody] UpdateFriendshipCommand updateFriendshipCommand,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(updateFriendshipCommand, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("count/status/{status}")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Count friendships by status",
        Description = "Returns the count of friendships with the specified status."
    )]
    public async Task<IActionResult> FilterByStatus([FromRoute] Status status, CancellationToken cancellationToken)
    {
        var query = new GetCountByStatusFriendshipQuery { Status = status };
        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("after-created")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get friendships created after a date",
        Description = "Retrieves friendships created after the specified date range."
    )]
    public async Task<IActionResult> GetAfterCreatedAsync([FromQuery] DateRangeType dateRange,
        CancellationToken cancellationToken)
    {
        var query = new GetCreatedAfterFriendshipQuery { DateRange = dateRange };
        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("before-created")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get friendships created before a date",
        Description = "Retrieves friendships created before the specified past date range."
    )]
    public async Task<IActionResult> GetBeforeCreatedAsync([FromQuery] PastDateRangeType pastDateRange,
        CancellationToken cancellationToken)
    {
        var query = new GetCreatedBeforeFriendshipQuery { PastDateRangeType = pastDateRange };
        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("filter-status")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Filter friendships by status",
        Description = "Retrieves friendships filtered by the specified status."
    )]
    public async Task<IActionResult> GetFilterByStatus([FromQuery] Status status, CancellationToken cancellationToken)
    {
        var query = new FilterByStatusFriendshipQuery { Status = status };
        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("order-by-id")]
    [DisableRateLimiting]
    [SwaggerOperation(
        Summary = "Order friendships by ID",
        Description = "Retrieves friendships ordered ascending or descending by ID."
    )]
    public async Task<IActionResult> OrderbyIdAscOrDescAsync([FromQuery] string sort,
        CancellationToken cancellationToken)
    {
        var query = new GetOrderByIdFriendshipQuery { Sort = sort };
        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("recent-created/{limit}")]
    [DisableRateLimiting]
    [SwaggerOperation(
        Summary = "Get recently created friendships",
        Description = "Retrieves the most recently created friendships up to the specified limit."
    )]
    public async Task<IActionResult> GetRecentCreatedAsync([FromRoute] int limit, CancellationToken cancellationToken)
    {
        var query = new GetRecentlyCreatedFriendshipQuery { Limit = limit };
        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("pagination")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get paginated friendships",
        Description = "Retrieves a paginated list of friendships based on the specified page number and size."
    )]
    public async Task<IActionResult> GetPagedAsync([FromQuery] int pageNumber, [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new GetPagedFriendshipQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("{friendshipId}/with-users")]
    [SwaggerOperation(
        Summary = "Get friendship with users",
        Description = "Retrieves the friendship along with the associated users."
    )]
    public async Task<IActionResult> GetWithUsersAsync([FromRoute] Guid friendshipId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetFriendshipWithUsersQuery { FriendshipId = friendshipId },
            cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Value);
    }
}