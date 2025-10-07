using Asp.Versioning;
using MediatR;
using MetaBond.Application.DTOs.ProgressBoard;
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
using MetaBond.Application.Helpers;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using MetaBond.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

namespace MetaBond.Presentation.Api.Controllers.V1;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:ApiVersion}/progress-board")]
public class ProgressBoardController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Create a new progress board",
        Description = "Creates a new progress board using the provided command data."
    )]
    public async Task<ResultT<ProgressBoardDTos>> CreateAsync([FromBody] CreateProgressBoardCommand command,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(command, cancellationToken);
    }

    [HttpPut]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Update a progress board",
        Description = "Updates an existing progress board with the provided data."
    )]
    public async Task<ResultT<ProgressBoardDTos>> UpdateAsync(
        [FromBody] UpdateProgressBoardCommand updateProgressBoardCommand,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(updateProgressBoardCommand, cancellationToken);
    }

    [HttpDelete("{id}")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Delete a progress board",
        Description = "Deletes a progress board by its unique ID."
    )]
    public async Task<ResultT<Guid>> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new DeleteProgressBoardCommand { ProgressBoardId = id };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("{id}")]
    [DisableRateLimiting]
    [SwaggerOperation(
        Summary = "Get a progress board by ID",
        Description = "Retrieves a progress board using its unique ID."
    )]
    public async Task<ResultT<ProgressBoardDTos>> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetByIdProgressBoardQuery { ProgressBoardId = id };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("count")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get total count of progress boards",
        Description = "Returns the total number of progress boards available."
    )]
    public async Task<ResultT<int>> GetCountAsync(CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetCountProgressBoardQuery(), cancellationToken);
    }

    [HttpGet("{id}/progress-entries")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get progress entries of a board",
        Description = "Retrieves paginated progress entries for a specific progress board by ID."
    )]
    public async Task<ResultT<PagedResult<ProgressBoardWithProgressEntryDTos>>> GetProgressEntriesAsync(
        [FromRoute] Guid id, [FromQuery] int pageNumber,
        [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        var query = new GetProgressBoardIdWithEntriesQuery
        {
            ProgressBoardId = id,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("filter/by-date")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Filter progress boards by date",
        Description = "Retrieves progress boards within a specific date range using pagination."
    )]
    public async Task<ResultT<PagedResult<ProgressBoardWithProgressEntryDTos>>> GetFilterDateRangeAsync(
        [FromQuery] DateRangeType dateRange, [FromQuery] int page,
        [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        var query = new GetRangeProgressBoardQuery
        {
            Page = page,
            PageSize = pageSize,
            DateRangeType = dateRange
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("recent-entries")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get recent progress entries",
        Description = "Retrieves the most recent progress entries filtered by a date range."
    )]
    public async Task<ResultT<PagedResult<ProgressBoardDTos>>> GetFilterRecentAsync(
        [FromQuery] DateRangeFilter dateRange,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new GetRecentProgressBoardQuery
        {
            DateFilter = dateRange,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("pagination")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get paginated progress boards",
        Description = "Retrieves progress boards using pagination parameters: pageNumber and pageSize."
    )]
    public async Task<ResultT<PagedResult<ProgressBoardDTos>>> GetPagedAsync([FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new GetPagedProgressBoardQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("{progressBoardId}/with-author")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get progress board with author",
        Description = "Retrieves a progress board along with its author information."
    )]
    public async Task<ResultT<PagedResult<ProgressBoardWithUserDTos>>> GetProgressBoardWithAuthorAsync(
        [FromRoute] Guid progressBoardId,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetProgressBoardsWithAuthorQuery
            {
                ProgressBoardId = progressBoardId,
                PageNumber = pageNumber,
                PageSize = pageSize
            },
            cancellationToken);
    }
}