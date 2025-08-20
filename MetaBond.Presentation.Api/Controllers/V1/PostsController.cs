using Asp.Versioning;
using MediatR;
using MetaBond.Application.Feature.Posts.Commands.Create;
using MetaBond.Application.Feature.Posts.Commands.Delete;
using MetaBond.Application.Feature.Posts.Query.GetById;
using MetaBond.Application.Feature.Posts.Query.GetFilterRecent;
using MetaBond.Application.Feature.Posts.Query.GetFilterTitle;
using MetaBond.Application.Feature.Posts.Query.GetFilterTop10;
using MetaBond.Application.Feature.Posts.Query.GetPostByIdCommunities;
using MetaBond.Application.Feature.Posts.Query.GetPostWithAuthor;
using MetaBond.Application.Feature.Posts.Query.Pagination;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

namespace MetaBond.Presentation.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:ApiVersion}/posts")]
public class PostsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [DisableRateLimiting]
    [SwaggerOperation(
        Summary = "Create a new post",
        Description = "Creates a new post using form data."
    )]
    public async Task<IActionResult> AddPostsAsync([FromForm] CreatePostsCommand createPostsCommand,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(createPostsCommand, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpDelete("{id}")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Delete a post",
        Description = "Deletes a post identified by its ID."
    )]
    public async Task<IActionResult> DeletePostsAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new DeletePostsCommand { PostsId = id };
        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("{id}")]
    [DisableRateLimiting]
    [SwaggerOperation(
        Summary = "Get a post by ID",
        Description = "Retrieves a post by its unique ID."
    )]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetByIdPostsQuery { PostsId = id };
        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("communities/{communitiesId}/recent-posts")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get recent posts in a community",
        Description = "Retrieves the most recent posts in a specific community. Specify topCount to limit results."
    )]
    public async Task<IActionResult> GetRecentPostsAsync([FromRoute] Guid communitiesId, [FromQuery] int topCount,
        CancellationToken cancellationToken)
    {
        if (topCount <= 0)
            return BadRequest("The topCount parameter must be greater than zero.");

        var query = new GetFilterRecentPostsQuery
        {
            CommunitiesId = communitiesId,
            TopCount = topCount
        };

        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("communities/{communitiesId}/recent-posts-top10")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get top 10 recent posts in a community",
        Description = "Retrieves the 10 most recent posts for a specific community."
    )]
    public async Task<IActionResult> GetRecentTop10Posts([FromRoute] Guid communitiesId,
        CancellationToken cancellationToken)
    {
        var query = new GetFilterTop10Query { CommunitiesId = communitiesId };
        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("{postsId}/details/communities")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get post details with communities",
        Description = "Retrieves detailed information of a post along with the communities it belongs to."
    )]
    public async Task<IActionResult> GetDetailsPosts([FromRoute] Guid postsId, CancellationToken cancellationToken)
    {
        var query = new GetPostsByIdCommunitiesQuery { PostsId = postsId };
        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("communities/{communitiesId}/search/title")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Filter posts by title",
        Description = "Retrieves posts in a community filtered by title keyword."
    )]
    public async Task<IActionResult> FilterByTitleAsync([FromRoute] Guid communitiesId, [FromQuery] string title,
        CancellationToken cancellationToken)
    {
        var query = new GetFilterTitlePostsQuery
        {
            CommunitiesId = communitiesId,
            Title = title
        };

        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("pagination")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get paginated posts",
        Description = "Retrieves posts using pagination parameters: pageNumber and pageSize."
    )]
    public async Task<IActionResult> GetPagedResultAsync([FromQuery] int pageNumber, [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new GetPagedPostsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("{postsId}/with-author")]
    [SwaggerOperation(
        Summary = "Get post with author info",
        Description = "Retrieves a post along with its author details."
    )]
    public async Task<IActionResult> GetPostsWithAuthorAsync([FromRoute] Guid postsId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetPostWithAuthorQuery { PostsId = postsId }, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Value);
    }
}