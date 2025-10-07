using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Pagination;
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
        : IQueryHandler<GetRecentProgressBoardQuery, PagedResult<ProgressBoardDTos>>
    {
        public async Task<ResultT<PagedResult<ProgressBoardDTos>>> Handle(
            GetRecentProgressBoardQuery request,
            CancellationToken cancellationToken)
        {
            var paginationValidation = PaginationHelper.ValidatePagination<ProgressBoardDTos>(
                request.PageNumber,
                request.PageSize,
                logger
            );

            if (!paginationValidation.IsSuccess) return paginationValidation.Error!;

            var progressBoardDictionary = GetValue(request.PageNumber, request.PageSize);
            if (progressBoardDictionary.TryGetValue((request.DateFilter), out var statusFilter))
            {
                var progressBoardFilter = await decoratedCache.GetOrCreateAsync(
                    $"progress-board-get-recent-{request.DateFilter}",
                    async () =>
                    {
                        var progressBoards = await statusFilter(cancellationToken);

                        var items = progressBoards.Items ?? [];

                        var progressBoardDTos = items.Select(x => new ProgressBoardDTos
                        (
                            ProgressBoardId: x.Id,
                            CommunitiesId: x.CommunitiesId,
                            UserId: x.UserId,
                            CreatedAt: x.CreatedAt,
                            UpdatedAt: x.UpdatedAt
                        ));

                        PagedResult<ProgressBoardDTos> pagedResult = new(
                            totalItems: progressBoards.TotalItems,
                            items: progressBoardDTos,
                            currentPage: progressBoards.CurrentPage,
                            pageSize: request.PageSize
                        );

                        return pagedResult;
                    },
                    cancellationToken: cancellationToken);

                var itemsDto = progressBoardFilter.Items ?? [];

                if (!itemsDto.Any())
                {
                    logger.LogWarning("No progress boards found for the given date filter: {DateFilter}",
                        request.DateFilter);

                    return ResultT<PagedResult<ProgressBoardDTos>>.Failure(Error.Failure("400",
                        "No progress boards found"));
                }

                logger.LogInformation("Retrieved {Count} progress boards for date filter: {DateFilter}",
                    itemsDto.Count(), request.DateFilter);

                return ResultT<PagedResult<ProgressBoardDTos>>.Success(progressBoardFilter);
            }

            logger.LogError("Invalid date filter provided: {DateFilter}", request.DateFilter);

            return ResultT<PagedResult<ProgressBoardDTos>>.Failure(Error.Failure("400", "Invalid date filter"));
        }

        #region Private Methods

        private Dictionary<DateRangeFilter, Func<CancellationToken, Task<PagedResult<Domain.Models.ProgressBoard>>>>
            GetValue(int pageNumber, int pageSize)
        {
            return new Dictionary<DateRangeFilter,
                Func<CancellationToken, Task<PagedResult<Domain.Models.ProgressBoard>>>>
            {
                {
                    (DateRangeFilter.LastDay),
                    async cancellationToken =>
                        await progressBoardRepository.GetRecentBoardsAsync(DateTime.UtcNow.AddDays(-1),
                            pageNumber,
                            pageSize,
                            cancellationToken)
                },
                {
                    (DateRangeFilter.ThreeDays),
                    async cancellationToken =>
                        await progressBoardRepository.GetRecentBoardsAsync(DateTime.UtcNow.AddTicks(-3),
                            pageNumber,
                            pageSize,
                            cancellationToken)
                },
                {
                    (DateRangeFilter.LastWeek),
                    async cancellationToken =>
                        await progressBoardRepository.GetRecentBoardsAsync(DateTime.UtcNow.AddDays(-5),
                            pageNumber,
                            pageSize,
                            cancellationToken)
                },
            };
        }

        #endregion
    }
}