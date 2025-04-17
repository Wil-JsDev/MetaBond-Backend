using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressEntry.Query.GetDateRange;

internal sealed class GetEntriesByDateRangeQueryHandler(
    IProgressEntryRepository progressEntryRepository,
    IDistributedCache decoratedCache,
    ILogger<GetEntriesByDateRangeQueryHandler> logger)
    : IQueryHandler<GetEntriesByDateRangeQuery, IEnumerable<ProgressEntryDTos>>
{
    public async Task<ResultT<IEnumerable<ProgressEntryDTos>>> Handle(
        GetEntriesByDateRangeQuery request, 
        CancellationToken cancellationToken)
    {
        var progressEntry = GetValue(request.ProgressBoardId);
        if (progressEntry.TryGetValue((request.Range), out var dateRange))
        {
            var progressEntryList = await decoratedCache.GetOrCreateAsync(
                $"progress-entry-get-by-date-range-{request.Range}",
                async () => await dateRange(cancellationToken), 
                cancellationToken: cancellationToken);
            
            if (!progressEntry.Any())
            {
                logger.LogError("No progress entries found for the specified date range.");

                return ResultT<IEnumerable<ProgressEntryDTos>>.Failure(Error.Failure("400", "No progress entries available for the selected date range."));
            }

            IEnumerable<ProgressEntryDTos> entriesDtos = progressEntryList.Select(x => new ProgressEntryDTos
            (
                ProgressEntryId: x.Id,
                ProgressBoardId: x.ProgressBoardId,
                Description: x.Description,
                CreatedAt: x.CreatedAt,
                UpdateAt: x.UpdateAt
            ));

            IEnumerable<ProgressEntryDTos> progressEntryDTosEnumerable = entriesDtos.ToList();
            logger.LogInformation("Successfully retrieved {Count} progress entries for the date range {Range}.", 
                progressEntryDTosEnumerable.Count(), request.Range);

            return ResultT<IEnumerable<ProgressEntryDTos>>.Success(progressEntryDTosEnumerable); 
        }

        logger.LogError("Invalid date range type provided: {Range}", request.Range);

        return ResultT<IEnumerable<ProgressEntryDTos>>.Failure(Error.Failure("400", "Invalid date range type."));
    }

    #region Private Methods
    private Dictionary<DateRangeType, Func<CancellationToken,Task<IEnumerable<Domain.Models.ProgressEntry>>>> GetValue(Guid progressBoardId)
    {
        return new Dictionary<DateRangeType, Func<CancellationToken, Task<IEnumerable<Domain.Models.ProgressEntry>>>>
        {
            { DateRangeType.Today, 
                async cancellationToken => 
                    await progressEntryRepository.GetEntriesByDateRangeAsync(
                        progressBoardId,
                        DateTime.UtcNow.Date, 
                        DateTime.UtcNow.AddDays(1).AddTicks(-1),
                        cancellationToken
                    ) },
            {DateRangeType.Week, 
                async cancellationToken => 
                    await progressEntryRepository.GetEntriesByDateRangeAsync(
                        progressBoardId,
                        DateTime.UtcNow.Date.AddDays(-7),
                        DateTime.UtcNow.Date.AddTicks(-7),
                        cancellationToken
                    )},
            {DateRangeType.Month,
                async cancellationToken => 
                    await progressEntryRepository.GetEntriesByDateRangeAsync(
                        progressBoardId,
                        new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1), //Primer dia del año
                        new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(1).AddTicks(-1), // Último día del mes actual (23:59:59.9999999)
                        cancellationToken
                    )},
            {DateRangeType.Year,
                async cancellationToken =>
                    await progressEntryRepository.GetEntriesByDateRangeAsync(
                        progressBoardId,
                        new DateTime(DateTime.UtcNow.Year, 1, 1), // Primer día del año actual
                        new DateTime(DateTime.UtcNow.Year + 1, 1, 1).AddTicks(-1), // Último día del año actual (23:59:59.9999999)
                        cancellationToken
                    )},
        };
    }
    
    #endregion
    
}