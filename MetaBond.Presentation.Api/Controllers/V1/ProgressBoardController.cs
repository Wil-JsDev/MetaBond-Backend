using Asp.Versioning;
using MediatR;
using MetaBond.Application.Feature.ProgressBoard.Commands.Create;
using MetaBond.Application.Feature.ProgressBoard.Commands.Delete;
using MetaBond.Application.Feature.ProgressBoard.Commands.Update;
using MetaBond.Application.Feature.ProgressBoard.Query.GetById;
using MetaBond.Application.Feature.ProgressBoard.Query.GetCount;
using MetaBond.Application.Feature.ProgressBoard.Query.GetProgressBoardsWithAuthor;
using MetaBond.Application.Feature.ProgressBoard.Query.GetProgressEntries;
using MetaBond.Application.Feature.ProgressBoard.Query.GetRange;
using MetaBond.Application.Feature.ProgressBoard.Query.GetRecent;
using MetaBond.Application.Feature.ProgressBoard.Query.Pagination;
using MetaBond.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

namespace MetaBond.Presentation.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:ApiVersion}/progress-board")]
public class ProgressBoardController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [DisableRateLimiting]
    [SwaggerOperation(
        Summary = "Create a new progress board",
        Description = "Creates a new progress board using the provided command data."
    )]
    public async Task<IActionResult> CreateAsync([FromBody] CreateProgressBoardCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPut]
    [DisableRateLimiting]
    [SwaggerOperation(
        Summary = "Update a progress board",
        Description = "Updates an existing progress board with the provided data."
    )]
    public async Task<IActionResult> UpdateAsync([FromBody] UpdateProgressBoardCommand updateProgressBoardCommand,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(updateProgressBoardCommand, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpDelete("{id}")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Delete a progress board",
        Description = "Deletes a progress board by its unique ID."
    )]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new DeleteProgressBoardCommand { ProgressBoardId = id };
        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("{id}")]
    [DisableRateLimiting]
    [SwaggerOperation(
        Summary = "Get a progress board by ID",
        Description = "Retrieves a progress board using its unique ID."
    )]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetByIdProgressBoardQuery { ProgressBoardId = id };
        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("count")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get total count of progress boards",
        Description = "Returns the total number of progress boards available."
    )]
    public async Task<IActionResult> GetCountAsync(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetCountProgressBoardQuery(), cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("{id}/progress-entries")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get progress entries of a board",
        Description = "Retrieves paginated progress entries for a specific progress board by ID."
    )]
    public async Task<IActionResult> GetProgressEntriesAsync([FromRoute] Guid id, [FromQuery] int pageNumber,
        [FromQuery] int pageSize, CancellationToken cancellationToken)
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
    [SwaggerOperation(
        Summary = "Filter progress boards by date",
        Description = "Retrieves progress boards within a specific date range using pagination."
    )]
    public async Task<IActionResult> GetFilterDateRangeAsync([FromQuery] DateRangeType dateRange, [FromQuery] int page,
        [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        var query = new GetRangeProgressBoardQuery
        {
            Page = page,
            PageSize = pageSize,
            DateRangeType = dateRange
        };

        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("recent-entries")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get recent progress entries",
        Description = "Retrieves the most recent progress entries filtered by a date range."
    )]
    public async Task<IActionResult> GetFilterRecentAsync([FromQuery] DateRangeFilter dateRange,
        CancellationToken cancellationToken)
    {
        var query = new GetRecentProgressBoardQuery { DateFilter = dateRange };
        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("pagination")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get paginated progress boards",
        Description = "Retrieves progress boards using pagination parameters: pageNumber and pageSize."
    )]
    public async Task<IActionResult> GetPagedAsync([FromQuery] int pageNumber, [FromQuery] int pageSize,
        CancellationToken cancellationToken)
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

    [HttpGet("{progressBoardId}/with-author")]
    [SwaggerOperation(
        Summary = "Get progress board with author",
        Description = "Retrieves a progress board along with its author information."
    )]
    public async Task<IActionResult> GetProgressBoardWithAuthorAsync([FromRoute] Guid progressBoardId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetProgressBoardsWithAuthorQuery { ProgressBoardId = progressBoardId },
            cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Value);
    }
}