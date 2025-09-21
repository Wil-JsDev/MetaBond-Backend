using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressEntry.Query.GetDateRange;

internal sealed class GetEntriesByDateRangeQueryHandler(
    IProgressEntryRepository progressEntryRepository,
    IDistributedCache decoratedCache,
    ILogger<GetEntriesByDateRangeQueryHandler> logger)
    : IQueryHandler<GetEntriesByDateRangeQuery, PagedResult<ProgressEntryDTos>>
{
    public async Task<ResultT<PagedResult<ProgressEntryDTos>>> Handle(
        GetEntriesByDateRangeQuery request,
        CancellationToken cancellationToken)
    {
        var progressEntryResult = await EntityHelper.GetEntityByIdAsync(
            progressEntryRepository.GetByIdAsync,
            request.ProgressBoardId,
            "ProgressEntry",
            logger
        );

        if (!progressEntryResult.IsSuccess)
            return progressEntryResult.Error!;

        var progressEntryQueries = GetValue(progressEntryRepository,
            request.ProgressBoardId,
            request.PageNumber,
            request.PageSize);

        if (progressEntryQueries.TryGetValue((request.Range), out var dateRange))
        {
            var result = await decoratedCache.GetOrCreateAsync(
                $"progress-entry-get-by-date-range-{request.Range}",
                async () =>
                {
                    var progressEntryList = await dateRange(cancellationToken);

                    var items = progressEntryList.Items ?? [];

                    var progressEntryDTos = items.Select(ProgressEntryMapper.ToDto);

                    PagedResult<ProgressEntryDTos> pagedResult = new()
                    {
                        CurrentPage = progressEntryList.CurrentPage,
                        TotalItems = progressEntryList.TotalItems,
                        TotalPages = progressEntryList.TotalPages,
                        Items = progressEntryDTos
                    };

                    return pagedResult;
                },
                cancellationToken: cancellationToken);

            var itemsDto = result.Items ?? [];

            if (!itemsDto.Any())
            {
                logger.LogError("No progress entries found for the specified date range.");

                return ResultT<PagedResult<ProgressEntryDTos>>.Failure(Error.Failure("400",
                    "No progress entries available for the selected date range."));
            }

            logger.LogInformation("Successfully retrieved {Count} progress entries for the date range {Range}.",
                itemsDto.Count(), request.Range);

            return ResultT<PagedResult<ProgressEntryDTos>>.Success(result);
        }

        logger.LogError("Invalid date range type provided: {Range}", request.Range);

        return ResultT<PagedResult<ProgressEntryDTos>>.Failure(Error.Failure("400", "Invalid date range type."));
    }

    #region Private Methods

    private static Dictionary<DateRangeType, Func<CancellationToken, Task<PagedResult<Domain.Models.ProgressEntry>>>>
        GetValue(
            IProgressEntryRepository progressEntryRepository,
            Guid progressBoardId,
            int pageNumber,
            int pageSize)
    {
        DateTime utcNow = DateTime.UtcNow.Date;

        DateTime StartOfDay() => utcNow;
        DateTime EndOfDay() => utcNow.AddDays(1).AddTicks(-1);

        DateTime StartOfWeek() => utcNow.AddDays(-7);
        DateTime EndOfWeek() => utcNow.AddDays(-1).AddTicks(-1);

        DateTime StartOfMonth() => new DateTime(utcNow.Year, utcNow.Month, 1);
        DateTime EndOfMonth() => StartOfMonth().AddMonths(1).AddTicks(-1);

        DateTime StartOfYear() => new DateTime(utcNow.Year, 1, 1);
        DateTime EndOfYear() => new DateTime(utcNow.Year + 1, 1, 1).AddTicks(-1);

        return new Dictionary<DateRangeType, Func<CancellationToken, Task<PagedResult<Domain.Models.ProgressEntry>>>>
        {
            {
                DateRangeType.Today,
                async cancellationToken =>
                    await progressEntryRepository.GetEntriesByDateRangeAsync(
                        progressBoardId,
                        StartOfDay(),
                        EndOfDay(),
                        pageNumber,
                        pageSize,
                        cancellationToken
                    )
            },
            {
                DateRangeType.Week,
                async cancellationToken =>
                    await progressEntryRepository.GetEntriesByDateRangeAsync(
                        progressBoardId,
                        StartOfWeek(),
                        EndOfWeek(),
                        pageNumber,
                        pageSize,
                        cancellationToken
                    )
            },
            {
                DateRangeType.Month,
                async cancellationToken =>
                    await progressEntryRepository.GetEntriesByDateRangeAsync(
                        progressBoardId,
                        StartOfMonth(),
                        EndOfMonth(),
                        pageNumber,
                        pageSize,
                        cancellationToken
                    )
            },
            {
                DateRangeType.Year,
                async cancellationToken =>
                    await progressEntryRepository.GetEntriesByDateRangeAsync(
                        progressBoardId,
                        StartOfYear(),
                        EndOfYear(),
                        pageNumber,
                        pageSize,
                        cancellationToken
                    )
            }
        };
    }

    #endregion
}