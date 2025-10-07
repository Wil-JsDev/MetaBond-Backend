using Asp.Versioning;
using MediatR;
using MetaBond.Application.DTOs.Posts;
using MetaBond.Application.Feature.Posts.Commands.Create;
using MetaBond.Application.Feature.Posts.Commands.Delete;
using MetaBond.Application.Feature.Posts.Query.GetById;
using MetaBond.Application.Feature.Posts.Query.GetFilterRecent;
using MetaBond.Application.Feature.Posts.Query.GetFilterTitle;
using MetaBond.Application.Feature.Posts.Query.GetFilterTop10;
using MetaBond.Application.Feature.Posts.Query.GetPostByIdCommunities;
using MetaBond.Application.Feature.Posts.Query.GetPostWithAuthor;
using MetaBond.Application.Feature.Posts.Query.Pagination;
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
[Route("api/v{version:ApiVersion}/posts")]
public class PostsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Create a new post",
        Description = "Creates a new post using form data."
    )]
    public async Task<ResultT<PostsDTos>> AddPostsAsync([FromForm] CreatePostsCommand createPostsCommand,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(createPostsCommand, cancellationToken);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = $"{UserRoleNames.Admin}, {UserRoleNames.User}")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Delete a post",
        Description = "Deletes a post identified by its ID."
    )]
    public async Task<ResultT<Guid>> DeletePostsAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new DeletePostsCommand { PostsId = id };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("{id}")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get a post by ID",
        Description = "Retrieves a post by its unique ID."
    )]
    public async Task<ResultT<PostsDTos>> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetByIdPostsQuery { PostsId = id };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("communities/{communitiesId}/recent-posts")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get recent posts in a community",
        Description = "Retrieves the most recent posts in a specific community. Specify topCount to limit results."
    )]
    public async Task<ResultT<PagedResult<PostsDTos>>> GetRecentPostsAsync([FromRoute] Guid communitiesId,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new GetFilterRecentPostsQuery
        {
            CommunitiesId = communitiesId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("communities/{communitiesId}/recent-posts-top10")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get top 10 recent posts in a community",
        Description = "Retrieves the 10 most recent posts for a specific community."
    )]
    public async Task<ResultT<IEnumerable<PostsDTos>>> GetRecentTop10Posts([FromRoute] Guid communitiesId,
        CancellationToken cancellationToken)
    {
        var query = new GetFilterTop10Query { CommunitiesId = communitiesId };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("{postsId}/details/communities")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get post details with communities",
        Description = "Retrieves detailed information of a post along with the communities it belongs to."
    )]
    public async Task<ResultT<PagedResult<PostsWithCommunitiesDTos>>> GetDetailsPosts([FromRoute] Guid postsId,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new GetPostsByIdCommunitiesQuery
        {
            PostsId = postsId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("communities/{communitiesId}/search/title")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Filter posts by title",
        Description = "Retrieves posts in a community filtered by title keyword."
    )]
    public async Task<ResultT<PagedResult<PostsDTos>>> FilterByTitleAsync([FromRoute] Guid communitiesId,
        [FromQuery] string title,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new GetFilterTitlePostsQuery
        {
            CommunitiesId = communitiesId,
            Title = title,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("pagination")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get paginated posts",
        Description = "Retrieves posts using pagination parameters: pageNumber and pageSize."
    )]
    public async Task<ResultT<PagedResult<PostsDTos>>> GetPagedResultAsync([FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new GetPagedPostsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("{postsId}/with-author")]
    [Authorize]
    [SwaggerOperation(
        Summary = "Get post with author info",
        Description = "Retrieves a post along with its author details."
    )]
    public async Task<ResultT<PagedResult<PostsWithUserDTos>>> GetPostsWithAuthorAsync([FromRoute] Guid postsId,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetPostWithAuthorQuery
        {
            PostsId = postsId,
            PageNumber = pageNumber,
            PageSize = pageSize
        }, cancellationToken);
    }
}