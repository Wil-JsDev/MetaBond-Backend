using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
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
    : IQueryHandler<GetRangeProgressBoardQuery, IEnumerable<ProgressBoardWithProgressEntryDTos>>
{
    public async Task<ResultT<IEnumerable<ProgressBoardWithProgressEntryDTos>>> Handle(
        GetRangeProgressBoardQuery request,
        CancellationToken cancellationToken)
    {
        var paginationValidationResult = PaginationHelper.ValidatePagination<ProgressBoardWithProgressEntryDTos>
        (
            request.Page,
            request.PageSize,
            logger
        );
        
        if (!paginationValidationResult.IsSuccess)
            return paginationValidationResult.Error!;

        var progressBoard = GetValue();
        if (progressBoard.TryGetValue((request.DateRangeType), out var progressBoardValue))
        {
            var progressBoards = await progressBoardValue(cancellationToken);

            var boards = progressBoards.ToList();
            if (!boards.Any())
            {
                logger.LogError("No progress board entries found for DateRangeType: {DateRangeType}",
                    request.DateRangeType);

                var getMessage = GetMessage().TryGetValue(request.DateRangeType, out var messageValue);
                if (getMessage)
                {
                    logger.LogWarning("No progress board found for the given date range");

                    return ResultT<IEnumerable<ProgressBoardWithProgressEntryDTos>>.Failure(Error.NotFound("404",
                        messageValue!));
                }
            }

            if (request.Page <= 0 || request.PageSize <= 0)
            {
                logger.LogWarning("Invalid pagination parameters: Page = {Page}, PageSize = {PageSize}", request.Page,
                    request.PageSize);

                return ResultT<IEnumerable<ProgressBoardWithProgressEntryDTos>>.Failure(
                    Error.Failure("400",
                        "Page number and page size must be greater than zero. Please provide valid pagination values."));
            }

            var result = await decoratedCache.GetOrCreateAsync(
                $"progress-entry-paged-get-range-{request.Page}-{request.PageSize}",
                async () =>
                {
                    var progressEntryPaged = await progressEntryRepository.GetPagedProgressEntryAsync(
                        request.PageSize,
                        request.Page,
                        cancellationToken);

                    var progressEntries = progressEntryPaged.Items!.ToList();

                    var progressBoardWithProgressEntryDs = boards.Select(board =>
                        board.ToProgressBoardWithEntriesDto(progressEntries));

                    return progressBoardWithProgressEntryDs;
                }, cancellationToken: cancellationToken);

            var progressBoardWithProgressEntryDTosEnumerable = result.ToList();
            logger.LogInformation(
                "Successfully retrieved {Count} progress board entries for DateRangeType: {DateRangeType}",
                progressBoardWithProgressEntryDTosEnumerable.Count(), request.DateRangeType);

            return ResultT<IEnumerable<ProgressBoardWithProgressEntryDTos>>.Success(
                progressBoardWithProgressEntryDTosEnumerable);
        }

        logger.LogError("Invalid DateRangeType: {DateRangeType}. No matching progress board found.",
            request.DateRangeType);

        return ResultT<IEnumerable<ProgressBoardWithProgressEntryDTos>>.Failure(
            Error.Failure("400", "Invalid DateRangeType provided"));
    }

    #region Private Methods

    private static Dictionary<DateRangeType, string> GetMessage()
    {
        return new Dictionary<DateRangeType, string>
        {
            { DateRangeType.Today, "No records available for today." },
            { DateRangeType.Week, "No records available for this week." },
            { DateRangeType.Month, "No records available for this month." },
            { DateRangeType.Year, "No records available for this year." },
        };
    }

    private Dictionary<DateRangeType, Func<CancellationToken, Task<IEnumerable<Domain.Models.ProgressBoard>>>>
        GetValue()
    {
        return new Dictionary<DateRangeType, Func<CancellationToken, Task<IEnumerable<Domain.Models.ProgressBoard>>>>
        {
            {
                DateRangeType.Today,
                async cancellationToken =>
                    await progressBoardRepository.GetBoardsByDateRangeAsync(
                        DateTime.SpecifyKind(DateTime.UtcNow.Date, DateTimeKind.Utc), // We ensure that the date is UTC
                        DateTime.SpecifyKind(DateTime.UtcNow.AddDays(1).AddTicks(-1),
                            DateTimeKind.Utc), // End of today in UTC
                        cancellationToken)
            },

            {
                DateRangeType.Week,
                async cancellationToken =>
                    await progressBoardRepository.GetBoardsByDateRangeAsync(
                        DateTime.SpecifyKind(DateTime.UtcNow.Date.AddDays(-7),
                            DateTimeKind.Utc), // Start of last week in UTC
                        DateTime.SpecifyKind(DateTime.UtcNow.Date.AddDays(-1),
                            DateTimeKind.Utc), // End of last week in UTC
                        cancellationToken)
            },

            {
                DateRangeType.Month,
                async cancellationToken =>
                    await progressBoardRepository.GetBoardsByDateRangeAsync(
                        DateTime.SpecifyKind(new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1),
                            DateTimeKind.Utc), // Start of the month in UTC
                        DateTime.SpecifyKind(
                            new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(1).AddTicks(-1),
                            DateTimeKind.Utc), // End of the month in UTC
                        cancellationToken)
            },

            {
                DateRangeType.Year,
                async cancellationToken =>
                    await progressBoardRepository.GetBoardsByDateRangeAsync(
                        DateTime.SpecifyKind(new DateTime(DateTime.UtcNow.Year, 1, 1),
                            DateTimeKind.Utc), // Start of the year in UTC
                        DateTime.SpecifyKind(new DateTime(DateTime.UtcNow.Year + 1, 1, 1).AddTicks(-1),
                            DateTimeKind.Utc), // End of the year in UTC
                        cancellationToken)
            }
        };
    }

    #endregion
}