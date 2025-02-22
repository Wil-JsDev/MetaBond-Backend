using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressBoard.Querys.GetRecent
{
    internal sealed class GetRecentProgressBoardQuerysHandler : IQueryHandler<GetRecentProgressBoardQuerys, IEnumerable<ProgressBoardDTos>>
    {
        private readonly IProgressBoardRepository _progressBoardRepository;
        private readonly ILogger<GetRecentProgressBoardQuerysHandler> _logger;

        public GetRecentProgressBoardQuerysHandler(
            IProgressBoardRepository progressBoardRepository, 
            ILogger<GetRecentProgressBoardQuerysHandler> logger)
        {
            _progressBoardRepository = progressBoardRepository;
            _logger = logger;
        }

        public async Task<ResultT<IEnumerable<ProgressBoardDTos>>> Handle(
            GetRecentProgressBoardQuerys request, 
            CancellationToken cancellationToken)
        {
            var progressBoardDictionary = GetValue();
            if (progressBoardDictionary.TryGetValue((request.dateFilter), out var statusFilter))
            {
                var progressBoard = await statusFilter(cancellationToken);
                if (progressBoard == null || !progressBoard.Any())
                {
                    _logger.LogWarning("No progress boards found for the given date filter: {DateFilter}", request.dateFilter);

                    return ResultT<IEnumerable<ProgressBoardDTos>>.Failure(Error.Failure("400", "No progress boards found"));
                }

                IEnumerable<ProgressBoardDTos> progressBoardDTos = progressBoard.Select(x => new ProgressBoardDTos
                (
                    ProgressBoardId: x.Id,
                    CommunitiesId: x.CommunitiesId,
                    CreatedAt: x.CreatedAt,
                    UpdatedAt: x.UpdatedAt
                ));

                _logger.LogInformation("Retrieved {Count} progress boards for date filter: {DateFilter}", progressBoardDTos.Count(), request.dateFilter);

                return ResultT<IEnumerable<ProgressBoardDTos>>.Success(progressBoardDTos);
            }
            _logger.LogError("Invalid date filter provided: {DateFilter}", request.dateFilter);

            return ResultT<IEnumerable<ProgressBoardDTos>>.Failure(Error.Failure("400", "Invalid date filter"));
        }

        #region Private Methods
        private Dictionary<DateRangeFilter, Func<CancellationToken, Task<IEnumerable<Domain.Models.ProgressBoard>>>> GetValue()
        {
            return new Dictionary<DateRangeFilter, Func<CancellationToken, Task<IEnumerable<Domain.Models.ProgressBoard>>>>
            {
                {(DateRangeFilter.LastDay), async cancellationToken => await _progressBoardRepository.GetRecentBoardsAsync(DateTime.UtcNow.AddDays(-1),cancellationToken)},
                {(DateRangeFilter.ThreeDays), async cancellationToken => await _progressBoardRepository.GetRecentBoardsAsync(DateTime.UtcNow.AddTicks(-3),cancellationToken)},
                {(DateRangeFilter.LastWeek), async cancellationToken => await _progressBoardRepository.GetRecentBoardsAsync(DateTime.UtcNow.AddDays(-5),cancellationToken)},

            };
        }
        #endregion

    }
}
