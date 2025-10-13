using Asp.Versioning;
using MediatR;
using MetaBond.Application.DTOs.Friendship;
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
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

namespace MetaBond.Presentation.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/{version:ApiVersion}/friendship")]
public class FriendshipController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Create a new friendship",
        Description = "Creates a new friendship using the provided command data."
    )]
    public async Task<ResultT<FriendshipDTos>> CreateAsync([FromBody] CreateFriendshipCommand friendshipCommand,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(friendshipCommand, cancellationToken);
    }

    [HttpGet("{id}")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get friendship by ID",
        Description = "Retrieves a specific friendship by its unique identifier."
    )]
    public async Task<ResultT<FriendshipDTos>> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetByIdFriendshipQuery { Id = id };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpDelete("{id}")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Delete a friendship by ID",
        Description = "Deletes the friendship identified by the given ID."
    )]
    public async Task<ResultT<Guid>> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new DeleteFriendshipCommand { Id = id };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpPut]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Update a friendship",
        Description = "Updates an existing friendship using the provided command data."
    )]
    public async Task<ResultT<UpdateFriendshipDTos>> UpdateAsync(
        [FromBody] UpdateFriendshipCommand updateFriendshipCommand,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(updateFriendshipCommand, cancellationToken);
    }

    [HttpGet("count/status/{status}")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Count friendships by status",
        Description = "Returns the count of friendships with the specified status."
    )]
    public async Task<ResultT<int>> FilterByStatus([FromRoute] Status status, CancellationToken cancellationToken)
    {
        var query = new GetCountByStatusFriendshipQuery { Status = status };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("after-created")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get friendships created after a date",
        Description = "Retrieves friendships created after the specified date range."
    )]
    public async Task<ResultT<PagedResult<FriendshipDTos>>> GetAfterCreatedAsync([FromQuery] DateRangeType dateRange,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new GetCreatedAfterFriendshipQuery
        {
            DateRange = dateRange,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("before-created")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get friendships created before a date",
        Description = "Retrieves friendships created before the specified past date range."
    )]
    public async Task<ResultT<PagedResult<FriendshipDTos>>> GetBeforeCreatedAsync(
        [FromQuery] PastDateRangeType pastDateRange,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new GetCreatedBeforeFriendshipQuery
        {
            PastDateRangeType = pastDateRange,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("filter-status")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Filter friendships by status",
        Description = "Retrieves friendships filtered by the specified status."
    )]
    public async Task<ResultT<PagedResult<FriendshipDTos>>> GetFilterByStatus([FromQuery] Status status,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new FilterByStatusFriendshipQuery
        {
            Status = status,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("order-by-id")]
    [DisableRateLimiting]
    [SwaggerOperation(
        Summary = "Order friendships by ID",
        Description = "Retrieves friendships ordered ascending or descending by ID."
    )]
    public async Task<ResultT<PagedResult<FriendshipDTos>>> OrderbyIdAscOrDescAsync([FromQuery] string sort,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new GetOrderByIdFriendshipQuery
        {
            Sort = sort,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("recent-created/{limit}")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get recently created friendships",
        Description = "Retrieves the most recently created friendships up to the specified limit."
    )]
    public async Task<ResultT<PagedResult<FriendshipDTos>>> GetRecentCreatedAsync([FromRoute] int limit,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new GetRecentlyCreatedFriendshipQuery
        {
            Limit = limit,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("pagination")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get paginated friendships",
        Description = "Retrieves a paginated list of friendships based on the specified page number and size."
    )]
    public async Task<ResultT<PagedResult<FriendshipDTos>>> GetPagedAsync([FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new GetPagedFriendshipQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("{friendshipId}/with-users")]
    [SwaggerOperation(
        Summary = "Get friendship with users",
        Description = "Retrieves the friendship along with the associated users."
    )]
    public async Task<ResultT<PagedResult<FriendshipWithUserDTos>>> GetWithUsersAsync([FromRoute] Guid friendshipId,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetFriendshipWithUsersQuery
            {
                FriendshipId = friendshipId,
                PageNumber = pageNumber,
                PageSize = pageSize
            },
            cancellationToken);
    }
}