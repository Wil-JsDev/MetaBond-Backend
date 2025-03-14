﻿using Asp.Versioning;
using MediatR;
using MetaBond.Application.Feature.ProgressBoard.Commands.Create;
using MetaBond.Application.Feature.ProgressBoard.Commands.Delete;
using MetaBond.Application.Feature.ProgressBoard.Commands.Update;
using MetaBond.Application.Feature.ProgressBoard.Querys.GetById;
using MetaBond.Application.Feature.ProgressBoard.Querys.GetCount;
using MetaBond.Application.Feature.ProgressBoard.Querys.GetProgressEntries;
using MetaBond.Application.Feature.ProgressBoard.Querys.GetRange;
using MetaBond.Application.Feature.ProgressBoard.Querys.GetRecent;
using MetaBond.Application.Feature.ProgressBoard.Querys.Pagination;
using MetaBond.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace MetaBond.Presentation.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:ApiVersion}/progress-board")]
    public class ProgressBoardController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ProgressBoardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [DisableRateLimiting]
        public async Task<IActionResult> CreateAsync([FromBody] Guid communitiesId,CancellationToken cancellationToken)
        {
            var query = new CreateProgressBoardCommand { CommunitiesId = communitiesId };

            var result = await _mediator.Send(query,cancellationToken);
            if(!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpPut("{id}")]
        [DisableRateLimiting]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid id,[FromBody] UpdateProgressBoardCommand updateProgressBoardCommand,CancellationToken cancellationToken)
        {
            updateProgressBoardCommand.CommunitiesId = id;

            var result = await _mediator.Send(updateProgressBoardCommand,cancellationToken);
            if(!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpDelete("{id}")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id,CancellationToken cancellationToken)
        {
            var query = new DeleteProgressBoardCommand { ProgressBoardId = id };

            var result = await _mediator.Send(query, cancellationToken);
            if(!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        [DisableRateLimiting]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id,CancellationToken cancellationToken)
        {
            var query = new GetByIdProgressBoardQuerys { ProgressBoardId= id };

            var result = await _mediator.Send(query,cancellationToken);
            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("count")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetCountAsync(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetCountProgressBoardQuerys(),cancellationToken);
            if(!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("{id}/progress-entries")]
        [DisableRateLimiting]
        public async Task<IActionResult> GetProgressEntriesdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var query = new GetProgressBoardIdWithEntriesQuerys { ProgressBoardId = id };

            var result = await _mediator.Send(query, cancellationToken);
            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value); 
        }

        [HttpGet("by-date-range")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetFilterDateRangeAsync([FromQuery] DateRangeType dateRange,CancellationToken cancellationToken)
        {
            var query = new GetRangeProgressBoardQuerys { DateRangeType = dateRange };

            var result = await _mediator.Send(query,cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("recent-entries")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetFilterRecentAsync([FromQuery] DateRangeFilter dateRange,CancellationToken cancellationToken)
        {
            var query = new GetRecentProgressBoardQuerys { dateFilter = dateRange };

            var result = await _mediator.Send(query, cancellationToken);
            if (!result.IsSuccess) 
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("pagination")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> GetPagedAsync([FromQuery] int pageNumber, [FromQuery] int pageSize,CancellationToken cancellationToken)
        {
            var query = new GetPagedProgressBoardQuerys
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
