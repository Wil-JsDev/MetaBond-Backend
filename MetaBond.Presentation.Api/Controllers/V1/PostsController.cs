using Asp.Versioning;
using MediatR;
using MetaBond.Application.Feature.Posts.Commands.Create;
using MetaBond.Application.Feature.Posts.Commands.Delete;
using MetaBond.Application.Feature.Posts.Querys.GetById;
using MetaBond.Application.Feature.Posts.Querys.GetFilterRecent;
using MetaBond.Application.Feature.Posts.Querys.GetFilterTitle;
using MetaBond.Application.Feature.Posts.Querys.GetFilterTop10;
using MetaBond.Application.Feature.Posts.Querys.GetPostByIdCommunities;
using MetaBond.Application.Feature.Posts.Querys.Pagination;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace MetaBond.Presentation.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:ApiVersion}/posts")]
    public class PostsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PostsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [DisableRateLimiting]
        public async Task<IActionResult> AddPostsAsync([FromForm] CreatePostsCommand createPostsCommand, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(createPostsCommand,cancellationToken);
            if(!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpDelete("{id}")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> DeletePostsAsync([FromRoute] Guid id,CancellationToken cancellationToken)
        {
            var query = new DeletePostsCommand { PostsId = id };
            var result = await _mediator.Send(query,cancellationToken);
            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        [DisableRateLimiting]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var query = new GetByIdPostsQuerys { PostsId= id };

            var result = await _mediator.Send(query, cancellationToken);
            if (!result.IsSuccess)
                return NotFound(result.Error);
            
            return Ok(result.Value);
        }

        [HttpGet("recent-posts")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetRecentPostsAsync([FromQuery] int topCount,CancellationToken cancellationToken)
        {
            if (topCount <= 0)
                return BadRequest("The topCount parameter must be greater than zero.");

            var query = new GetFilterRecentPostsQuery { TopCount = topCount };

            var result = await _mediator.Send(query, cancellationToken);
            if (!result.IsSuccess) 
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("recent-posts-top10")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetRecentTop10Posts(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send( new GetFilterTop10Query(),cancellationToken);
            if (!result.IsSuccess)
                    return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("{id}/details/communities")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetDetailsPosts([FromRoute] Guid id,CancellationToken cancellationToken)
        {
            var query = new GetPostsByIdCommunitiesQuery { PostsId = id };
            var result = await _mediator.Send(query,cancellationToken);
            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("filter-by-title")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> FilterByTitleAsync([FromQuery] string title,CancellationToken cancellationToken)
        {
            var query = new GetFilterTitlePostsQuery { Title = title };

            var result = await _mediator.Send(query,cancellationToken);
            if(!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("pagination")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetPagedResultAsync([FromQuery] int pageNumber, [FromQuery] int pageSize,CancellationToken cancellationToken)
        {
            var query = new GetPagedPostsQuerys
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }


    }
}
