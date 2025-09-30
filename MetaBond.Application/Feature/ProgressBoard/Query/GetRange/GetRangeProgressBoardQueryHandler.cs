using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressBoard.Query.GetRange;

internal sealed class GetRangeProgressBoardQueryHandler(
    IProgressBoardRepository progressBoardRepository,
    IProgressEntryRepository progressEntryRepository,
    IDistributedCache decoratedCache,
    ILogger<GetRangeProgressBoardQueryHandler> logger)
    : IQueryHandler<GetRangeProgressBoardQuery, PagedResult<ProgressBoardWithProgressEntryDTos>>
{
    public async Task<ResultT<PagedResult<ProgressBoardWithProgressEntryDTos>>> Handle(
        GetRangeProgressBoardQuery request,
        CancellationToken cancellationToken)
    {
        var paginationValidationResult = PaginationHelper.ValidatePagination<ProgressBoardWithProgressEntryDTos>(
            request.Page,
            request.PageSize,
            logger
        );

        if (!paginationValidationResult.IsSuccess)
            return paginationValidationResult.Error!;

        if (!TryGetBoardsQuery(request.DateRangeType, request.Page, request.PageSize, out var boardsQuery))
        {
            logger.LogError("Invalid DateRangeType: {DateRangeType}. No matching progress board found.",
                request.DateRangeType);

            return ResultT<PagedResult<ProgressBoardWithProgressEntryDTos>>.Failure(
                Error.Failure("400", "Invalid DateRangeType provided"));
        }

        var cacheKey = $"progress-entry-paged-get-range-{request.DateRangeType}-{request.Page}-{request.PageSize}";

        var result = await decoratedCache.GetOrCreateAsync(
            cacheKey,
            async () =>
            {
                var progressBoards = await boardsQuery(cancellationToken);
                var boards = progressBoards.Items?.ToList() ?? [];

                if (!boards.Any())
                {
                    if (Messages.TryGetValue(request.DateRangeType, out var messageValue))
                    {
                        logger.LogWarning("No progress board found for DateRangeType: {DateRangeType}",
                            request.DateRangeType);

                        return ResultT<PagedResult<ProgressBoardWithProgressEntryDTos>>.Failure(
                            Error.Failure("400", messageValue));
                    }
                }

                var progressEntryPaged = await progressEntryRepository.GetPagedProgressEntryAsync(
                    request.PageSize,
                    request.Page,
                    cancellationToken);

                var progressEntries = progressEntryPaged.Items ?? [];

                var boardDtos = boards
                    .Select(board => board.ToProgressBoardWithEntriesDto(progressEntries))
                    .ToList();

                return new PagedResult<ProgressBoardWithProgressEntryDTos>
                {
                    TotalItems = progressBoards.TotalItems,
                    CurrentPage = progressBoards.CurrentPage,
                    TotalPages = progressBoards.TotalPages,
                    Items = boardDtos
                };
            },
            cancellationToken: cancellationToken);

        logger.LogInformation(
            "Successfully retrieved {Count} progress board entries for DateRangeType: {DateRangeType}",
            result.Value.Items!.Count(), request.DateRangeType);

        return ResultT<PagedResult<ProgressBoardWithProgressEntryDTos>>.Success(result.Value);
    }

    #region Private Helpers

    private static readonly Dictionary<DateRangeType, string> Messages = new()
    {
        { DateRangeType.Today, "No records available for today." },
        { DateRangeType.Week, "No records available for this week." },
        { DateRangeType.Month, "No records available for this month." },
        { DateRangeType.Year, "No records available for this year." },
    };

    private static (DateTime Start, DateTime End) GetDateRange(DateRangeType type)
    {
        var now = DateTime.UtcNow.Date;

        return type switch
        {
            DateRangeType.Today => (now, now.AddDays(1).AddTicks(-1)),
            DateRangeType.Week => (now.AddDays(-7), now.AddDays(-1)),
            DateRangeType.Month => (new DateTime(now.Year, now.Month, 1),
                new DateTime(now.Year, now.Month, 1).AddMonths(1).AddTicks(-1)),
            DateRangeType.Year => (new DateTime(now.Year, 1, 1),
                new DateTime(now.Year + 1, 1, 1).AddTicks(-1)),
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
    }

    private bool TryGetBoardsQuery(
        DateRangeType rangeType,
        int page,
        int pageSize,
        out Func<CancellationToken, Task<PagedResult<Domain.Models.ProgressBoard>>> query)
    {
        try
        {
            var (start, end) = GetDateRange(rangeType);

            query = async cancellationToken =>
                await progressBoardRepository.GetBoardsByDateRangeAsync(
                    start,
                    end,
                    page,
                    pageSize,
                    cancellationToken);

            return true;
        }
        catch
        {
            query = null!;
            return false;
        }
    }

    #endregion
}