using Asp.Versioning;
using MediatR;
using MetaBond.Application.DTOs.InterestCategory;
using MetaBond.Application.Feature.InterestCategory.Command.Create;
using MetaBond.Application.Feature.InterestCategory.Command.Update;
using MetaBond.Application.Feature.InterestCategory.Query.GetById;
using MetaBond.Application.Feature.InterestCategory.Query.GetByName;
using MetaBond.Application.Feature.InterestCategory.Query.Pagination;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using MetaBond.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

namespace MetaBond.Presentation.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:ApiVersion}/interest-categories")]
public class InterestCategoryController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [EnableRateLimiting("fixed")]
    [Authorize(Roles = UserRoleNames.Admin)]
    [SwaggerOperation(
        Summary = "Create a new interest category",
        Description = "Creates a new interest category. Only users with Admin role are allowed."
    )]
    public async Task<ResultT<InterestCategoryDTos>> CreateAsync(
        [FromBody] CreateInterestCategoryCommand command,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(command, cancellationToken);
    }

    [HttpPut("{interestCategoryId}")]
    [Authorize(Roles = UserRoleNames.Admin)]
    [SwaggerOperation(
        Summary = "Update an existing interest category",
        Description = "Updates the name of an existing interest category. Only Admin users can perform this operation."
    )]
    public async Task<ResultT<UpdateInterestCategoryDTos>> UpdateAsync(
        [FromRoute] Guid interestCategoryId,
        [FromBody] ParameterInterestCategory interestCategory,
        CancellationToken cancellationToken)
    {
        var command = new UpdateInterestCategoryCommand()
        {
            Name = interestCategory.Name,
            InterestCategoryId = interestCategoryId
        };

        return await mediator.Send(command, cancellationToken);
    }

    [HttpGet("{interestCategoryId}")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get interest category by ID",
        Description = "Retrieves an interest category by its unique identifier. This endpoint is public."
    )]
    public async Task<ResultT<InterestCategoryGeneralDTos>> GetByIdAsync(
        [FromRoute] Guid interestCategoryId,
        CancellationToken cancellationToken)
    {
        var query = new GetByIdInterestCategoryQuery
        {
            InterestCategoryId = interestCategoryId
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("search/{name}")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get interest category by name",
        Description = "Searches for an interest category by name. This endpoint is public."
    )]
    public async Task<ResultT<InterestCategoryGeneralDTos>> GetByNameAsync(
        [FromRoute] string name,
        CancellationToken cancellationToken)
    {
        var query = new GetByNameInterestCategoryQuery()
        {
            Name = name
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("pagination")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get paginated interest categories",
        Description =
            "Retrieves interest categories in a paginated format. You can specify page number and page size. Public endpoint."
    )]
    public async Task<ResultT<PagedResult<InterestCategoryGeneralDTos>>> GetPagedAsync(
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken
    )
    {
        var query = new GetPagedInterestCategoryQuery()
        {
            PageSize = pageSize,
            PageNumber = pageNumber
        };

        return await mediator.Send(query, cancellationToken);
    }
}