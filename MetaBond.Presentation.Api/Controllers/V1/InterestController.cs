using Asp.Versioning;
using MediatR;
using MetaBond.Application.DTOs.Interest;
using MetaBond.Application.Feature.Interest.Commands.Create;
using MetaBond.Application.Feature.Interest.Query.GetById;
using MetaBond.Application.Feature.Interest.Query.GetInterestsByName;
using MetaBond.Application.Feature.Interest.Query.GetInterestsByUser;
using MetaBond.Application.Feature.Interest.Query.Pagination;
using MetaBond.Application.Pagination;
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
    [SwaggerOperation(
        Summary = "Create a new interest",
        Description = "Creates a new interest and returns the created resource."
    )]
    public async Task<IActionResult> CreateInterestAsync([FromBody] CreateInterestCommand command)
    {
        var result = await mediator.Send(command);

        return result.IsSuccess ? Ok(result.Value) : StatusCode(Convert.ToUInt16(result.Error!.Code), result.Error);
    }

    [HttpGet("{interestId}")]
    [SwaggerOperation(
        Summary = "Get interest by ID",
        Description = "Retrieves an interest by its unique identifier."
    )]
    public async Task<IActionResult> GetInterestByIdAsync([FromRoute] Guid interestId,
        CancellationToken cancellationToken)
    {
        var query = new GetByIdInterestQuery
        {
            InterestId = interestId
        };

        var result = await mediator.Send(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : StatusCode(Convert.ToUInt16(result.Error!.Code), result.Error);
    }

    [HttpGet("by-name")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Search interests by name",
        Description = "Retrieves a paginated list of interests matching the provided name."
    )]
    [ProducesResponseType(typeof(PagedResult<InterestDTos>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInterestByNameAsync([FromQuery] string interestName, int pageNumber,
        int pageSize, CancellationToken cancellationToken)
    {
        GetInterestByNameQuery query = new()
        {
            InterestName = interestName,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await mediator.Send(query, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : StatusCode(Convert.ToUInt16(result.Error!.Code), result.Error);
    }

    [HttpGet("by-user/{userId}")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get interests by user",
        Description = "Retrieves a paginated list of interests associated with a specific user."
    )]
    public async Task<IActionResult> GetInterestByUserAsync([FromRoute] Guid userId, [FromQuery] int pageNumber,
        [FromQuery] int pageSize, CancellationToken cancellationToken = default)
    {
        var query = new GetInterestByUserQuery
        {
            UserId = userId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await mediator.Send(query, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : StatusCode(Convert.ToInt32(result.Error!.Code), result.Error);
    }

    [HttpGet("pagination")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get paginated interests",
        Description = "Retrieves a paginated list of all interests."
    )]
    public async Task<IActionResult> GetPagedInterestAsync(int pageNumber,
        int pageSize, CancellationToken cancellationToken)
    {
        GetPagedInterestQuery query = new()
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await mediator.Send(query, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : StatusCode(Convert.ToUInt16(result.Error!.Code), result.Error);
    }
}