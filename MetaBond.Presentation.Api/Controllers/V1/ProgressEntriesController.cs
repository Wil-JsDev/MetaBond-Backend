using Asp.Versioning;
using MediatR;
using MetaBond.Application.Feature.ProgressEntry.Commands.Create;
using MetaBond.Application.Feature.ProgressEntry.Commands.Delete;
using MetaBond.Application.Feature.ProgressEntry.Commands.Update;
using MetaBond.Application.Feature.ProgressEntry.Querys.GetById;
using MetaBond.Application.Feature.ProgressEntry.Querys.GetCountByBoard;
using MetaBond.Application.Feature.ProgressEntry.Querys.GetDateRange;
using MetaBond.Application.Feature.ProgressEntry.Querys.GetOrderByDescription;
using MetaBond.Application.Feature.ProgressEntry.Querys.GetOrderById;
using MetaBond.Application.Feature.ProgressEntry.Querys.GetRecent;
using MetaBond.Application.Feature.ProgressEntry.Querys.Pagination;
using MetaBond.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace MetaBond.Presentation.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:ApiVersion}/progress-entries")]
    public class ProgressEntriesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ProgressEntriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateProgressEntryCommand createProgress,CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(createProgress,cancellationToken);
            if(!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpPut("{id}")]
        [DisableRateLimiting]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateProgressEntryCommand updateProgressEntry,CancellationToken cancellationToken)
        {
            updateProgressEntry.ProgressEntryId = id;

            var result = await _mediator.Send(updateProgressEntry,cancellationToken);
            if(!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpDelete("{id}")]
        [DisableRateLimiting]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var query = new DeleteProgressEntryCommand { Id = id };

            var result = await _mediator.Send(query, cancellationToken);
            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        [DisableRateLimiting]
        public  async Task<IActionResult> GetByIdAsync([FromRoute] Guid id,CancellationToken cancellationToken)
        {
            var query = new GetByIdProgressEntryQuery { ProgressEntryId = id };

            var result = await _mediator.Send(query,cancellationToken);
            if (!result.IsSuccess)
                    return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("count/{progressBoardId}")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetCountAsync([FromRoute] Guid progressBoardId, CancellationToken cancellationToken)
        {
            var query = new GetCountByBoardIdQuery { ProgressBoardId = progressBoardId };
            var result = await _mediator.Send(query,cancellationToken);
            if(!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("date-range")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetDateRangeAsync([FromQuery] DateRangeType dateRange,CancellationToken cancellationToken)
        {
            var query = new GetEntriesByDateRangeQuery { Range = dateRange };

            var result = await _mediator.Send(query,cancellationToken);
            if(!result.IsSuccess) 
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("recent")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetFilterByRecentAsync([FromQuery] int topCount,CancellationToken cancellationToken)
        {
            var query = new GetRecentEntriesQuery { TopCount = topCount };

            var result = await _mediator.Send(query,cancellationToken);
            if(!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("ordered-by-id")]
        [DisableRateLimiting]
        public async Task<IActionResult> OrderByIdAsync(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetOrderByIdProgressEntryQuery(),cancellationToken);
            if(!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("ordered-by-desciption")]
        [DisableRateLimiting]
        public async Task<IActionResult> OrderByDescriptionAsync(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetOrderByDescriptionProgressEntryQuery(),cancellationToken);
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

            var result = await _mediator.Send(query,cancellationToken);
            if(!result.IsSuccess) 
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

    }
}
