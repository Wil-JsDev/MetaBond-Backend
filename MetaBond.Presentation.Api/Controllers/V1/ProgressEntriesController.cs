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
using MetaBond.Application.Feature.ProgressEntry.Query.GetProgressEntriesWithAuthor;
using MetaBond.Application.Feature.ProgressEntry.Query.GetRecent;
using MetaBond.Application.Feature.ProgressEntry.Query.Pagination;
using MetaBond.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

namespace MetaBond.Presentation.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:ApiVersion}/progress-entries")]
public class ProgressEntriesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Create a new progress entry",
        Description = "Creates a new progress entry using the provided command data."
    )]
    public async Task<IActionResult> CreateAsync([FromBody] CreateProgressEntryCommand createProgress,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(createProgress, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPut]
    [DisableRateLimiting]
    [SwaggerOperation(
        Summary = "Update a progress entry",
        Description = "Updates an existing progress entry using the provided command data."
    )]
    public async Task<IActionResult> UpdateAsync([FromBody] UpdateProgressEntryCommand updateProgressEntry,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(updateProgressEntry, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpDelete("{id}")]
    [DisableRateLimiting]
    [SwaggerOperation(
        Summary = "Delete a progress entry",
        Description = "Deletes a progress entry by its unique ID."
    )]
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
    [SwaggerOperation(
        Summary = "Get a progress entry by ID",
        Description = "Retrieves a progress entry using its unique ID."
    )]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetByIdProgressEntryQuery { ProgressEntryId = id };
        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("count/{progressBoardId}")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get count of entries for a board",
        Description = "Returns the total number of progress entries for a given progress board."
    )]
    public async Task<IActionResult> GetCountAsync([FromRoute] Guid progressBoardId,
        CancellationToken cancellationToken)
    {
        var query = new GetCountByBoardIdQuery { ProgressBoardId = progressBoardId };
        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("progress-board/{progressBoardId}/date-range")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get progress entries by date range",
        Description = "Retrieves progress entries for a board filtered by a specified date range."
    )]
    public async Task<IActionResult> GetDateRangeAsync([FromRoute] Guid progressBoardId,
        [FromQuery] DateRangeType dateRange, CancellationToken cancellationToken)
    {
        var query = new GetEntriesByDateRangeQuery
        {
            ProgressBoardId = progressBoardId,
            Range = dateRange
        };
        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("progress-board/{progressBoardId}/recent")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get recent progress entries",
        Description = "Retrieves the most recent progress entries for a board, limited by topCount."
    )]
    public async Task<IActionResult> GetFilterByRecentAsync([FromRoute] Guid progressBoardId, [FromQuery] int topCount,
        CancellationToken cancellationToken)
    {
        var query = new GetRecentEntriesQuery
        {
            ProgressBoardId = progressBoardId,
            TopCount = topCount
        };
        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("progress-board/{progressBoardId}/ordered-by-id")]
    [DisableRateLimiting]
    [SwaggerOperation(
        Summary = "Order progress entries by ID",
        Description = "Retrieves progress entries for a board ordered by their ID."
    )]
    public async Task<IActionResult> OrderByIdAsync([FromRoute] Guid progressBoardId,
        CancellationToken cancellationToken)
    {
        var query = new GetOrderByIdProgressEntryQuery { ProgressBoardId = progressBoardId };
        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("progress-board/{progressBoardId}/ordered-by-description")]
    [DisableRateLimiting]
    [SwaggerOperation(
        Summary = "Order progress entries by description",
        Description = "Retrieves progress entries for a board ordered by their description."
    )]
    public async Task<IActionResult> OrderByDescriptionAsync([FromRoute] Guid progressBoardId,
        CancellationToken cancellationToken)
    {
        var query = new GetOrderByDescriptionProgressEntryQuery { ProgressBoardId = progressBoardId };
        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("{id}/progress-boards")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get progress entry with board details",
        Description = "Retrieves a progress entry along with its associated progress board."
    )]
    public async Task<IActionResult> GetProgressEntryWithBoard([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetProgressEntryWithBoardByIdQuery { ProgressEntryId = id };
        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("pagination")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get paginated progress entries",
        Description = "Retrieves progress entries using pagination parameters: pageNumber and pageSize."
    )]
    public async Task<IActionResult> GetPagedResultAsync([FromQuery] int pageNumber, [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new GetPagedProgressEntryQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var result = await mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("{progressEntriesId}/with-author")]
    [SwaggerOperation(
        Summary = "Get progress entries with author",
        Description = "Retrieves a progress entry along with its author information."
    )]
    public async Task<IActionResult> GetProgressEntriesWithAuthorAsync([FromRoute] Guid progressEntriesId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetProgressEntriesWithAuthorsQuery { ProgressEntryId = progressEntriesId },
            cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }
}