using Asp.Versioning;
using MediatR;
using MetaBond.Application.DTOs.Communities;
using MetaBond.Application.Feature.Communities.Commands.Create;
using MetaBond.Application.Feature.Communities.Commands.Delete;
using MetaBond.Application.Feature.Communities.Commands.Update;
using MetaBond.Application.Feature.Communities.Query.Filter;
using MetaBond.Application.Feature.Communities.Query.GetById;
using MetaBond.Application.Feature.Communities.Query.GetPostsAndEvents;
using MetaBond.Application.Feature.Communities.Query.Pagination;
using MetaBond.Application.Helpers;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

namespace MetaBond.Presentation.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:ApiVersion}/communities")]
public class CommunitiesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Create a new community",
        Description = "Creates a new community using the provided command data."
    )]
    public async Task<ResultT<CommunitiesDTos>> AddAsync([FromBody] CreateCommunitiesCommand createCommand,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(createCommand, cancellationToken);
    }

    [HttpDelete]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Delete a community",
        Description = "Deletes a community by its unique identifier."
    )]
    public async Task<ResultT<Guid>> DeleteAsync([FromQuery] Guid id, CancellationToken cancellationToken)
    {
        var query = new DeleteCommunitiesCommand { Id = id };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpPut]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Update a community",
        Description = "Updates the information of an existing community."
    )]
    public async Task<ResultT<CommunitiesDTos>> UpdateAsync([FromBody] UpdateCommunitiesCommand updateCommand,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(updateCommand, cancellationToken);
    }

    [HttpGet("{id}")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get community by ID",
        Description = "Retrieves a community by its unique identifier."
    )]
    public async Task<ResultT<CommunitiesDTos>> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetByIdCommunitiesQuery { Id = id };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("search/category/{category}")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Search communities by category",
        Description = "Retrieves a list of communities filtered by a specific category."
    )]
    public async Task<ResultT<PagedResult<CommunitiesDTos>>> GetCommunitiesByCategoryIdAsync([FromRoute] Guid categoryId,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new GetCommunitiesByCategoryIdQuery
        {
            CategoryId = categoryId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("{id}/details")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get detailed community information",
        Description = "Retrieves detailed information about a community, including pagination for related data."
    )]
    public async Task<ResultT<IEnumerable<PostsAndEventsDTos>>> GetCommunitiesDetailsAsync(
        [FromRoute] Guid id,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new GetCommunityDetailsByIdQuery
        {
            Id = id,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("pagination")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get paginated communities",
        Description = "Retrieves a paginated list of communities."
    )]
    public async Task<ResultT<PagedResult<CommunitiesDTos>>> GetPagedAsync([FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new GetPagedCommunitiesQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }
}