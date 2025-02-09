using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressEntry.Querys.GetDateRange
{
    internal sealed class GetEntriesByDateRangeQueryHandler : IQueryHandler<GetEntriesByDateRangeQuery, IEnumerable<ProgressEntryDTos>>
    {
        private readonly IProgressEntryRepository _progressEntryRepository;
        private readonly ILogger<GetEntriesByDateRangeQueryHandler> _logger;

        public GetEntriesByDateRangeQueryHandler(
            IProgressEntryRepository progressEntryRepository, 
            ILogger<GetEntriesByDateRangeQueryHandler> logger)
        {
            _progressEntryRepository = progressEntryRepository;
            _logger = logger;
        }

        public async Task<ResultT<IEnumerable<ProgressEntryDTos>>> Handle(
            GetEntriesByDateRangeQuery request, 
            CancellationToken cancellationToken)
        {
            var progressEntry = GetValue();
            if (progressEntry.TryGetValue((request.Range), out var dateRange))
            {
                var progressEntryList = await dateRange(cancellationToken);
                if (progressEntry == null || !progressEntry.Any())
                {
                    _logger.LogError("No progress entries found for the specified date range.");

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

                _logger.LogInformation("Successfully retrieved {Count} progress entries for the date range {Range}.", 
                    entriesDtos.Count(), request.Range);

                return ResultT<IEnumerable<ProgressEntryDTos>>.Success(entriesDtos); 
            }

            _logger.LogError("Invalid date range type provided: {Range}", request.Range);

            return ResultT<IEnumerable<ProgressEntryDTos>>.Failure(Error.Failure("400", "Invalid date range type."));
        }

        private Dictionary<DateRangeType, Func<CancellationToken,Task<IEnumerable<Domain.Models.ProgressEntry>>>> GetValue()
        {
            return new Dictionary<DateRangeType, Func<CancellationToken, Task<IEnumerable<Domain.Models.ProgressEntry>>>>
            {
                { DateRangeType.Today, 
                    async cancellationToken => 
                        await _progressEntryRepository.GetEntriesByDateRangeAsync(
                            DateTime.UtcNow.Date, 
                            DateTime.UtcNow.AddDays(1).AddTicks(-1),
                            cancellationToken
                        ) },
                {DateRangeType.Week, 
                    async cancellationToken => 
                        await _progressEntryRepository.GetEntriesByDateRangeAsync(
                            DateTime.UtcNow.Date.AddDays(-7),
                            DateTime.UtcNow.Date.AddTicks(-7),
                            cancellationToken
                        )},
                {DateRangeType.Month,
                    async cancellationToken => 
                        await _progressEntryRepository.GetEntriesByDateRangeAsync(
                            new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1), //Primer dia del año
                            new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(1).AddTicks(-1), // Último día del mes actual (23:59:59.9999999)
                            cancellationToken
                        )},
                {DateRangeType.Year,
                    async cancellationToken =>
                        await _progressEntryRepository.GetEntriesByDateRangeAsync(
                            new DateTime(DateTime.UtcNow.Year, 1, 1), // Primer día del año actual
                            new DateTime(DateTime.UtcNow.Year + 1, 1, 1).AddTicks(-1), // Último día del año actual (23:59:59.9999999)
                            cancellationToken
                        )},
            };
        }
    }
}
