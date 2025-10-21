using Asp.Versioning;
using MediatR;
using MetaBond.Application.DTOs.ProgressEntry;
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
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

namespace MetaBond.Presentation.Api.Controllers.V1;

[ApiController]
[Authorize]
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
    public async Task<ResultT<ProgressEntryDTos>> CreateAsync([FromBody] CreateProgressEntryCommand createProgress,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(createProgress, cancellationToken);
    }

    [HttpPut]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Update a progress entry",
        Description = "Updates an existing progress entry using the provided command data."
    )]
    public async Task<ResultT<ProgressEntryDTos>> UpdateAsync([FromBody] UpdateProgressEntryCommand updateProgressEntry,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(updateProgressEntry, cancellationToken);
    }

    [HttpDelete("{id}")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Delete a progress entry",
        Description = "Deletes a progress entry by its unique ID."
    )]
    public async Task<ResultT<Guid>> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new DeleteProgressEntryCommand { Id = id };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("{id}")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get a progress entry by ID",
        Description = "Retrieves a progress entry using its unique ID."
    )]
    public async Task<ResultT<ProgressEntryDTos>> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetByIdProgressEntryQuery { ProgressEntryId = id };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("count/{progressBoardId}")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get count of entries for a board",
        Description = "Returns the total number of progress entries for a given progress board."
    )]
    public async Task<ResultT<int>> GetCountAsync([FromRoute] Guid progressBoardId,
        CancellationToken cancellationToken)
    {
        var query = new GetCountByBoardIdQuery { ProgressBoardId = progressBoardId };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("progress-board/{progressBoardId}/date-range")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get progress entries by date range",
        Description = "Retrieves progress entries for a board filtered by a specified date range."
    )]
    public async Task<ResultT<PagedResult<ProgressEntryDTos>>> GetDateRangeAsync([FromRoute] Guid progressBoardId,
        [FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] DateRangeType dateRange,
        CancellationToken cancellationToken)
    {
        var query = new GetEntriesByDateRangeQuery
        {
            ProgressBoardId = progressBoardId,
            Range = dateRange,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("progress-board/{progressBoardId}/recent")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get recent progress entries",
        Description = "Retrieves the most recent progress entries for a board, limited by topCount."
    )]
    public async Task<ResultT<PagedResult<ProgressEntryDTos>>> GetFilterByRecentAsync([FromRoute] Guid progressBoardId,
        [FromQuery] int pageNumber, [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new GetRecentEntriesQuery
        {
            ProgressBoardId = progressBoardId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("progress-board/{progressBoardId}/ordered-by-id")]
    [DisableRateLimiting]
    [SwaggerOperation(
        Summary = "Order progress entries by ID",
        Description = "Retrieves progress entries for a board ordered by their ID."
    )]
    public async Task<ResultT<PagedResult<ProgressEntryBasicDTos>>> OrderByIdAsync([FromRoute] Guid progressBoardId,
        [FromQuery] int pageNumber, [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        var query = new GetOrderByIdProgressEntryQuery
        {
            ProgressBoardId = progressBoardId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("progress-board/{progressBoardId}/ordered-by-description")]
    [DisableRateLimiting]
    [SwaggerOperation(
        Summary = "Order progress entries by description",
        Description = "Retrieves progress entries for a board ordered by their description."
    )]
    public async Task<ResultT<PagedResult<ProgressEntryWithDescriptionDTos>>> OrderByDescriptionAsync(
        [FromRoute] Guid progressBoardId,
        [FromQuery] int pageNumber, [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new GetOrderByDescriptionProgressEntryQuery
        {
            ProgressBoardId = progressBoardId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("{id}/progress-boards")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get progress entry with board details",
        Description = "Retrieves a progress entry along with its associated progress board."
    )]
    public async Task<ResultT<PagedResult<ProgressEntryWithProgressBoardDTos>>> GetProgressEntryWithBoard(
        [FromRoute] Guid id, [FromQuery] int pageNumber, [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        var query = new GetProgressEntryWithBoardByIdQuery
        {
            ProgressEntryId = id,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("pagination")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get paginated progress entries",
        Description = "Retrieves progress entries using pagination parameters: pageNumber and pageSize."
    )]
    public async Task<ResultT<PagedResult<ProgressEntryDTos>>> GetPagedResultAsync([FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new GetPagedProgressEntryQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("{progressEntriesId}/with-author")]
    [SwaggerOperation(
        Summary = "Get progress entries with author",
        Description = "Retrieves a progress entry along with its author information."
    )]
    public async Task<ResultT<PagedResult<ProgressEntriesWithUserDTos>>> GetProgressEntriesWithAuthorAsync(
        [FromRoute] Guid progressEntriesId,
        [FromQuery] int pageNumber, [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetProgressEntriesWithAuthorsQuery
            {
                ProgressEntryId = progressEntriesId,
                PageNumber = pageNumber,
                PageSize = pageSize
            },
            cancellationToken);
    }
}