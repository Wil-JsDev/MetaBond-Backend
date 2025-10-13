using Asp.Versioning;
using MediatR;
using MetaBond.Application.DTOs.Interest;
using MetaBond.Application.Feature.Interest.Commands.Create;
using MetaBond.Application.Feature.Interest.Query.GetById;
using MetaBond.Application.Feature.Interest.Query.GetInterestByCategory;
using MetaBond.Application.Feature.Interest.Query.GetInterestsByName;
using MetaBond.Application.Feature.Interest.Query.GetInterestsByUser;
using MetaBond.Application.Feature.Interest.Query.Pagination;
using MetaBond.Application.Helpers;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using MetaBond.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

namespace MetaBond.Presentation.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/interests")]
public class InterestController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = UserRoleNames.Admin)]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Create a new interest",
        Description = "Creates a new interest and returns the created resource."
    )]
    public async Task<ResultT<InterestDTos>> CreateInterestAsync([FromBody] CreateInterestCommand command,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(command, cancellationToken);
    }

    [HttpGet("{interestId}")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get interest by ID",
        Description = "Retrieves an interest by its unique identifier."
    )]
    public async Task<ResultT<InterestDTos>> GetInterestByIdAsync([FromRoute] Guid interestId,
        CancellationToken cancellationToken)
    {
        var query = new GetByIdInterestQuery
        {
            InterestId = interestId
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("by-name")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Search interests by name",
        Description = "Retrieves a paginated list of interests matching the provided name."
    )]
    [ProducesResponseType(typeof(PagedResult<InterestDTos>), StatusCodes.Status200OK)]
    public async Task<ResultT<PagedResult<InterestWithUserDto>>> GetInterestByNameAsync([FromQuery] string interestName,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        GetInterestByNameQuery query = new()
        {
            InterestName = interestName,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("by-user/{userId}")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get interests by user",
        Description = "Retrieves a paginated list of interests associated with a specific user."
    )]
    public async Task<ResultT<PagedResult<InterestDTos>>> GetInterestByUserAsync([FromRoute] Guid userId,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize, CancellationToken cancellationToken = default)
    {
        var query = new GetInterestByUserQuery
        {
            UserId = userId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("pagination")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get paginated interests",
        Description = "Retrieves a paginated list of all interests."
    )]
    public async Task<ResultT<PagedResult<InterestDTos>>> GetPagedInterestAsync(
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        GetPagedInterestQuery query = new()
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("interest-category/{interestCategoryId}")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get interests by category",
        Description = "Retrieves a paginated list of interests associated with a specific category."
    )]
    public async Task<ResultT<PagedResult<InterestDTos>>> GetPagedInterestByInterestCategoryAsync(
        [FromQuery] List<Guid> interestCategoryId,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken
    )
    {
        var query = new GetInterestByCategoryIdQuery()
        {
            InterestCategoryId = interestCategoryId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }
}