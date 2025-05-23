﻿using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressBoard.Query.GetRecent
{
    internal sealed class GetRecentProgressBoardQueryHandler(
        IProgressBoardRepository progressBoardRepository,
        IDistributedCache decoratedCache,
        ILogger<GetRecentProgressBoardQueryHandler> logger)
        : IQueryHandler<GetRecentProgressBoardQuery, IEnumerable<ProgressBoardDTos>>
    {
        public async Task<ResultT<IEnumerable<ProgressBoardDTos>>> Handle(
            GetRecentProgressBoardQuery request, 
            CancellationToken cancellationToken)
        {
            var progressBoardDictionary = GetValue();
            if (progressBoardDictionary.TryGetValue((request.DateFilter), out var statusFilter))
            {
                var progressBoardFilter = await decoratedCache.GetOrCreateAsync(
                    $"progress-board-get-recent-{request.DateFilter}",
                    async () => await statusFilter(cancellationToken), 
                    cancellationToken: cancellationToken);
                
                IEnumerable<Domain.Models.ProgressBoard> progressBoards = progressBoardFilter.ToList();
                if (!progressBoards.Any())
                {
                    logger.LogWarning("No progress boards found for the given date filter: {DateFilter}", request.DateFilter);

                    return ResultT<IEnumerable<ProgressBoardDTos>>.Failure(Error.Failure("400", "No progress boards found"));
                }

                IEnumerable<ProgressBoardDTos> progressBoardDTos = progressBoards.Select(x => new ProgressBoardDTos
                (
                    ProgressBoardId: x.Id,
                    CommunitiesId: x.CommunitiesId,
                    UserId: x.UserId,
                    CreatedAt: x.CreatedAt,
                    UpdatedAt: x.UpdatedAt
                ));

                IEnumerable<ProgressBoardDTos> progressBoardDTosEnumerable = progressBoardDTos.ToList();
                
                logger.LogInformation("Retrieved {Count} progress boards for date filter: {DateFilter}", progressBoardDTosEnumerable.Count(), request.DateFilter);

                return ResultT<IEnumerable<ProgressBoardDTos>>.Success(progressBoardDTosEnumerable);
            }
            logger.LogError("Invalid date filter provided: {DateFilter}", request.DateFilter);

            return ResultT<IEnumerable<ProgressBoardDTos>>.Failure(Error.Failure("400", "Invalid date filter"));
        }

        #region Private Methods
        private Dictionary<DateRangeFilter, Func<CancellationToken, Task<IEnumerable<Domain.Models.ProgressBoard>>>> GetValue()
        {
            return new Dictionary<DateRangeFilter, Func<CancellationToken, Task<IEnumerable<Domain.Models.ProgressBoard>>>>
            {
                {(DateRangeFilter.LastDay), async cancellationToken => await progressBoardRepository.GetRecentBoardsAsync(DateTime.UtcNow.AddDays(-1),cancellationToken)},
                {(DateRangeFilter.ThreeDays), async cancellationToken => await progressBoardRepository.GetRecentBoardsAsync(DateTime.UtcNow.AddTicks(-3),cancellationToken)},
                {(DateRangeFilter.LastWeek), async cancellationToken => await progressBoardRepository.GetRecentBoardsAsync(DateTime.UtcNow.AddDays(-5),cancellationToken)},

            };
        }
        #endregion

    }
}
