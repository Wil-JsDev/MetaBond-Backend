using Asp.Versioning;
using MediatR;
using MetaBond.Application.DTOs.CommunityCategory;
using MetaBond.Application.Feature.CommunityCategory.Command.Create;
using MetaBond.Application.Feature.CommunityCategory.Command.Update;
using MetaBond.Application.Feature.CommunityCategory.Query.GetById;
using MetaBond.Application.Feature.CommunityCategory.Query.GetByName;
using MetaBond.Application.Feature.CommunityCategory.Query.Pagination;
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
[Route("api/v{version:ApiVersion}/community-categories")]
public class CommunityCategoryController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = UserRoleNames.Admin)]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Create a new Community Category",
        Description = "Creates a new community category with the provided details."
    )]
    [ProducesResponseType(typeof(CommunityCategoryDTos), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<ResultT<CommunityCategoryDTos>> CreateCommunityCategoryAsync(
        [FromBody] CreateCommunityCategoryCommand command, CancellationToken cancellationToken)
    {
        return await mediator.Send(command, cancellationToken);
    }

    [HttpPut]
    [Authorize(Roles = UserRoleNames.Admin)]
    [SwaggerOperation(
        Summary = "Update an existing Community Category",
        Description = "Updates the name or details of an existing community category."
    )]
    [ProducesResponseType(typeof(CommunityCategoryDTos), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<ResultT<CommunityCategoryDTos>> UpdateCommunityCategoryAsync(
        [FromBody] UpdateCommunityCategoryCommand command, CancellationToken cancellationToken)
    {
        return await mediator.Send(command, cancellationToken);
    }

    [HttpGet("{communityCategoryId}")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get Community Category by Id",
        Description = "Retrieves a specific community category using its unique identifier."
    )]
    [ProducesResponseType(typeof(CommunityCategoryDTos), 200)]
    [ProducesResponseType(typeof(string), 404)]
    public async Task<ResultT<CommunityCategoryDTos>> GetByIdCommunityCategoryAsync(
        [FromRoute] Guid communityCategoryId, CancellationToken cancellationToken)
    {
        var query = new GetByIdCommunityCategoryQuery
        {
            CommunityCategoryId = communityCategoryId
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("search/{name}")]
    [SwaggerOperation(
        Summary = "Get Community Category by Name",
        Description = "Retrieves a community category by its name."
    )]
    [ProducesResponseType(typeof(CommunityCategoryDTos), 200)]
    [ProducesResponseType(typeof(string), 404)]
    public async Task<ResultT<CommunityCategoryDTos>> GetByNameCommunityCategoryAsync(
        [FromRoute] string name,
        CancellationToken cancellationToken)
    {
        var query = new GetByNameCommunityCategoryQuery
        {
            Name = name
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("pagination")]
    [SwaggerOperation(
        Summary = "Get paginated Community Categories",
        Description = "Retrieves a paginated list of community categories with page number and size."
    )]
    [ProducesResponseType(typeof(PagedResult<CommunityCategoryDTos>), 200)]
    [ProducesResponseType(typeof(string), 404)]
    public async Task<ResultT<PagedResult<CommunityCategoryDTos>>> GetPagedCommunityCategoryAsync(
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken
    )
    {
        var query = new GetPagedCommunityCategoryQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }
}