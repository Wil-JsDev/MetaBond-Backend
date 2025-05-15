using Asp.Versioning;
using MediatR;
using MetaBond.Application.Feature.ProgressEntry.Commands.Create;
using MetaBond.Application.Feature.ProgressEntry.Commands.Delete;
using MetaBond.Application.Feature.ProgressEntry.Commands.Update;
using MetaBond.Application.Feature.ProgressEntry.Query.GetById;
using MetaBond.Application.Feature.ProgressEntry.Query.GetByIdProgressEntryWithProgressBoard;
using MetaBond.Application.Feature.ProgressEntry.Query.GetCountByBoard;
using MetaBond.Application.Feature.ProgressEntry.Query.GetDateRange;
using MetaBond.Application.Feature.ProgressEntry.Query.GetOrderByDescription;
using MetaBond.Application.Feature.ProgressEntry.Query.GetOrderById;
using MetaBond.Application.Feature.ProgressEntry.Query.GetRecent;
using MetaBond.Application.Feature.ProgressEntry.Query.Pagination;
using MetaBond.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace MetaBond.Presentation.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:ApiVersion}/progress-entries")]
    public class ProgressEntriesController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateProgressEntryCommand createProgress,CancellationToken cancellationToken)
        {
            var result = await mediator.Send(createProgress,cancellationToken);
            if(!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpPut]
        [DisableRateLimiting]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateProgressEntryCommand updateProgressEntry,CancellationToken cancellationToken)
        {
            var result = await mediator.Send(updateProgressEntry,cancellationToken);
            if(!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpDelete("{id}")]
        [DisableRateLimiting]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var query = new DeleteProgressEntryCommand { Id = id };

            var result = await mediator.Send(query, cancellationToken);
            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        [DisableRateLimiting]
        public  async Task<IActionResult> GetByIdAsync([FromRoute] Guid id,CancellationToken cancellationToken)
        {
            var query = new GetByIdProgressEntryQuery { ProgressEntryId = id };

            var result = await mediator.Send(query,cancellationToken);
            if (!result.IsSuccess)
                    return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("count/{progressBoardId}")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetCountAsync([FromRoute] Guid progressBoardId, CancellationToken cancellationToken)
        {
            var query = new GetCountByBoardIdQuery { ProgressBoardId = progressBoardId };
            var result = await mediator.Send(query,cancellationToken);
            if(!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("progress-board/{progressBoardId}/date-range")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetDateRangeAsync(
            [FromRoute] Guid progressBoardId,
            [FromQuery] DateRangeType dateRange,
            CancellationToken cancellationToken)
        {
            var query = new GetEntriesByDateRangeQuery
            {
                ProgressBoardId = progressBoardId,
                Range = dateRange
            };

            var result = await mediator.Send(query,cancellationToken);
            if(!result.IsSuccess) 
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("progress-board/{progressBoardId}/recent")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetFilterByRecentAsync(
            [FromRoute] Guid progressBoardId,
            [FromQuery] int topCount,
            CancellationToken cancellationToken)
        {
            var query = new GetRecentEntriesQuery
            {
                ProgressBoardId = progressBoardId,
                TopCount = topCount
            };

            var result = await mediator.Send(query,cancellationToken);
            if(!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("progress-board/{progressBoardId}/ordered-by-id")]
        [DisableRateLimiting]
        public async Task<IActionResult> OrderByIdAsync([FromRoute] Guid progressBoardId,CancellationToken cancellationToken)
        {
            var query = new GetOrderByIdProgressEntryQuery { ProgressBoardId = progressBoardId };
            var result = await mediator.Send(query,cancellationToken);
            if(!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("progress-board/{progressBoardId}/ordered-by-description")]
        [DisableRateLimiting]
        public async Task<IActionResult> OrderByDescriptionAsync([FromRoute] Guid progressBoardId,CancellationToken cancellationToken)
        {
            var query = new GetOrderByDescriptionProgressEntryQuery { ProgressBoardId = progressBoardId };
            
            var result = await mediator.Send(query,cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }
        
        [HttpGet("{id}/progress-boards")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetProgressEntryWithBoard([FromRoute] Guid id,CancellationToken cancellationToken)
        {
            var query = new GetProgressEntryWithBoardByIdQuery { ProgressEntryId = id };
            var result = await mediator.Send(query,cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.Error);
            
            return Ok(result.Value);
        }

        [HttpGet("pagination")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetPagedResultAsync([FromQuery] int pageNumber, [FromQuery] int pageSize,CancellationToken cancellationToken)
        {
            var query = new GetPagedProgressEntryQuery
            {
                PageNumber =  pageNumber,
                PageSize = pageSize
            };

            var result = await mediator.Send(query,cancellationToken);
            if(!result.IsSuccess) 
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

    }
}
