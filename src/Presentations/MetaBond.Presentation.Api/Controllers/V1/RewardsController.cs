using Asp.Versioning;
using MediatR;
using MetaBond.Application.DTOs.Rewards;
using MetaBond.Application.Feature.Rewards.Commands.Create;
using MetaBond.Application.Feature.Rewards.Commands.Delete;
using MetaBond.Application.Feature.Rewards.Commands.Update;
using MetaBond.Application.Feature.Rewards.Query.GetById;
using MetaBond.Application.Feature.Rewards.Query.GetCount;
using MetaBond.Application.Feature.Rewards.Query.GetRange;
using MetaBond.Application.Feature.Rewards.Query.GetRecent;
using MetaBond.Application.Feature.Rewards.Query.GetTop;
using MetaBond.Application.Feature.Rewards.Query.GetUsersByReward;
using MetaBond.Application.Feature.Rewards.Query.Pagination;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Service;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

namespace MetaBond.Presentation.Api.Controllers.V1;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:ApiVersion}/rewards")]
public class RewardsController(IMediator mediator, ICurrentService currentService) : ControllerBase
{
    [HttpPost]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Create a new reward",
        Description = "Creates a new reward using the provided command data."
    )]
    public async Task<ResultT<RewardsDTos>> AddAsync([FromBody] CreateRewardsParameter rewardsParameter,
        CancellationToken cancellationToken)
    {
        var rewardsCommand = new CreateRewardsCommand()
        {
            UserId = currentService.CurrentId,
            Description = rewardsParameter.Description,
            PointAwarded = rewardsParameter.PointAwarded
        };
        return await mediator.Send(rewardsCommand, cancellationToken);
    }

    [HttpPut]
    [DisableRateLimiting]
    [SwaggerOperation(
        Summary = "Update a reward",
        Description = "Updates an existing reward using the provided command data."
    )]
    public async Task<ResultT<RewardsDTos>> UpdateAsync([FromBody] UpdateRewardsCommand rewardsCommand,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(rewardsCommand, cancellationToken);
    }

    [HttpDelete("{id}")]
    [DisableRateLimiting]
    [SwaggerOperation(
        Summary = "Delete a reward",
        Description = "Deletes a reward by its unique ID."
    )]
    public async Task<ResultT<Guid>> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new DeleteRewardsCommand { RewardsId = id };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("{id}")]
    [DisableRateLimiting]
    [SwaggerOperation(
        Summary = "Get reward by ID",
        Description = "Retrieves a reward using its unique ID."
    )]
    public async Task<ResultT<RewardsDTos>> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetByIdRewardsQuery { RewardsId = id };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("count")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get total count of rewards",
        Description = "Returns the total number of rewards in the system."
    )]
    public async Task<ResultT<int>> GetCountByIdAsync(CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetCountRewardsQuery(), cancellationToken);
    }

    [HttpGet("filter-by-date-range")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get rewards by date range",
        Description = "Retrieves rewards filtered by a specified date range."
    )]
    public async Task<ResultT<PagedResult<RewardsDTos>>> GetDateRangeAsync([FromQuery] DateRangeType range,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new GetByDateRangeRewardQuery
        {
            Range = range,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("filter/recent")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get most recent rewards",
        Description = "Retrieves the most recent rewards added to the system."
    )]
    public async Task<ResultT<RewardsDTos>> GetRecentAsync(CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetMostRecentRewardsQuery(), cancellationToken);
    }

    [HttpGet("top-rewards-by-count")]
    [DisableRateLimiting]
    [SwaggerOperation(
        Summary = "Get top rewards by count",
        Description = "Retrieves the top rewards ordered by a count parameter."
    )]
    public async Task<ResultT<PagedResult<RewardsWithUserDTos>>> GetTopRewards([FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new GetTopRewardsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("pagination")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get paginated rewards",
        Description = "Retrieves rewards using pagination parameters: pageNumber and pageSize."
    )]
    public async Task<ResultT<PagedResult<RewardsDTos>>> GetPagedRewards([FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new GetPagedRewardsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("{rewardsId}/with-author")]
    [SwaggerOperation(
        Summary = "Get reward with author",
        Description = "Retrieves a reward along with its author information."
    )]
    public async Task<ResultT<PagedResult<RewardsWithUserDTos>>> GetWithAuthorAsync([FromRoute] Guid rewardsId,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetUsersByRewardIdQuery
        {
            RewardsId = rewardsId,
            PageNumber = pageNumber,
            PageSize = pageSize
        }, cancellationToken);
    }
}