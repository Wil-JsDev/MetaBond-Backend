using Asp.Versioning;
using MediatR;
using MetaBond.Application.Feature.Communities.Commands;
using MetaBond.Application.Feature.Communities.Commands.Delete;
using MetaBond.Application.Feature.Communities.Commands.Update;
using MetaBond.Application.Feature.Communities.Querys.Filter;
using MetaBond.Application.Feature.Communities.Querys.GetById;
using MetaBond.Application.Feature.Communities.Querys.GetPostsAndEvents;
using MetaBond.Application.Feature.Communities.Querys.Pagination;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace MetaBond.Presentation.Api.Controllers.V1
{

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:ApiVersion}/communities")]
    public class CommunitiesController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> AddAsync([FromBody] CreateCommuntiesCommand createCommand, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(createCommand,cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpDelete]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> DeleteAsync([FromQuery] Guid id,CancellationToken cancellationToken)
        {
            var query = new DeleteCommunitiesCommand { Id = id };
            var result = await mediator.Send(query, cancellationToken);
            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpPut("update/{id}")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateCommunitiesCommand updateCommand,CancellationToken cancellationToken)
        {
            var result = await mediator.Send(updateCommand,cancellationToken);
            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id,CancellationToken cancellationToken)
        {
            var query = new GetByIdCommunitiesQuery { Id = id };
            var result = await mediator.Send(query,cancellationToken);
            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("search/category/{category}")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> FilterByCategoryAsync([FromRoute] string category,CancellationToken cancellationToken)
        {
            var query = new FilterCommunitiesQuery { Category = category };
            var result = await mediator.Send(query,cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
           
        }

        [HttpGet("{id}/details")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetCommunitiesDetailsAsync([FromRoute] Guid id,[FromQuery] int pageNumber, [FromQuery] int pageSize,CancellationToken cancellationToken)
        {
            var query = new GetCommunityDetailsByIdQuery
            {
                Id = id,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var result = await mediator.Send(query, cancellationToken);
            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("pagination")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetPagedAsync([FromQuery] int pageNumber, [FromQuery] int pageSize, CancellationToken cancellationToken)
        {
            var query = new GetPagedCommunitiesQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await mediator.Send(query, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }
    }
}
