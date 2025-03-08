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
    public class CommunitiesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CommunitiesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> AddAsync([FromBody] CreateCommuntiesCommand createCommand, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(createCommand,cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpDelete("{id}")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid Id,CancellationToken cancellationToken)
        {
            var query = new DeleteCommunitiesCommand { Id = Id };
            var result = await _mediator.Send(query, cancellationToken);
            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpPut("{id}")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid Id, [FromBody] UpdateCommunitiesCommand updateCommand,CancellationToken cancellationToken)
        {
            updateCommand.Id = Id;
            var result = await _mediator.Send(updateCommand,cancellationToken);
            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid Id,CancellationToken cancellationToken)
        {
            var query = new GetByIdCommunitiesQuery { Id = Id };
            var result = await _mediator.Send(query,cancellationToken);
            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("filter/by-category")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> FilterByCategoryAsync([FromQuery] string category,CancellationToken cancellationToken)
        {
            var query = new FilterCommunitiesQuery { Category = category };
            var result = await _mediator.Send(query,cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
           
        }

        [HttpGet("{id}/details")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetCommunitiesDetailsAsync([FromRoute] Guid Id,[FromQuery] int pageNumber, [FromQuery] int pageSize,CancellationToken cancellationToken)
        {
            var query = new GetCommunityDetailsByIdQuery
            {
                Id = Id,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var result = await _mediator.Send(query, cancellationToken);
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

            var result = await _mediator.Send(query, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }
    }
}
