using Asp.Versioning;
using MediatR;
using MetaBond.Application.Feature.ProgressBoard.Commands.Create;
using MetaBond.Application.Feature.ProgressBoard.Commands.Delete;
using MetaBond.Application.Feature.ProgressBoard.Commands.Update;
using MetaBond.Application.Feature.ProgressBoard.Query.GetById;
using MetaBond.Application.Feature.ProgressBoard.Query.GetCount;
using MetaBond.Application.Feature.ProgressBoard.Query.GetProgressEntries;
using MetaBond.Application.Feature.ProgressBoard.Query.GetRange;
using MetaBond.Application.Feature.ProgressBoard.Query.GetRecent;
using MetaBond.Application.Feature.ProgressBoard.Query.Pagination;
using MetaBond.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace MetaBond.Presentation.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:ApiVersion}/progress-board")]
    public class ProgressBoardController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        [DisableRateLimiting]
        public async Task<IActionResult> CreateAsync([FromBody] CreateProgressBoardCommand command,CancellationToken cancellationToken)
        {
            var result = await mediator.Send(command,cancellationToken);
            if(!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpPut]
        [DisableRateLimiting]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateProgressBoardCommand updateProgressBoardCommand,CancellationToken cancellationToken)
        {
            var result = await mediator.Send(updateProgressBoardCommand,cancellationToken);
            if(!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpDelete("{id}")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id,CancellationToken cancellationToken)
        {
            var query = new DeleteProgressBoardCommand { ProgressBoardId = id };

            var result = await mediator.Send(query, cancellationToken);
            if(!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        [DisableRateLimiting]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id,CancellationToken cancellationToken)
        {
            var query = new GetByIdProgressBoardQuery { ProgressBoardId= id };

            var result = await mediator.Send(query,cancellationToken);
            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("count")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetCountAsync(CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetCountProgressBoardQuery(),cancellationToken);
            if(!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("{id}/progress-entries")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetProgressEntriesAsync(
            [FromRoute] Guid id, 
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize,
            CancellationToken cancellationToken)
        {
            var query = new GetProgressBoardIdWithEntriesQuery
            {
                ProgressBoardId = id,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await mediator.Send(query, cancellationToken);
            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value); 
        }

        [HttpGet("filter/by-date")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetFilterDateRangeAsync(
            [FromQuery] DateRangeType dateRange,
            [FromQuery] int page,
            [FromQuery] int pageSize,
            CancellationToken cancellationToken)
        {
            var query = new GetRangeProgressBoardQuery
            {
                Page = page,
                PageSize = pageSize,
                DateRangeType = dateRange
            };

            var result = await mediator.Send(query,cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("recent-entries")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetFilterRecentAsync([FromQuery] DateRangeFilter dateRange,CancellationToken cancellationToken)
        {
            var query = new GetRecentProgressBoardQuery { DateFilter = dateRange };

            var result = await mediator.Send(query, cancellationToken);
            if (!result.IsSuccess) 
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("pagination")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetPagedAsync([FromQuery] int pageNumber, [FromQuery] int pageSize,CancellationToken cancellationToken)
        {
            var query = new GetPagedProgressBoardQuery
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
