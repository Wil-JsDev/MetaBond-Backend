using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Rewards;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Rewards.Query.GetRange;

internal sealed class GetByDateRangeRewardQueryHandler(
    IRewardsRepository rewardsRepository,
    IDistributedCache decoratedCache,
    ILogger<GetByDateRangeRewardQueryHandler> logger)
    : IQueryHandler<GetByDateRangeRewardQuery, PagedResult<RewardsDTos>>
{
    public async Task<ResultT<PagedResult<RewardsDTos>>> Handle(
        GetByDateRangeRewardQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Range.ToString()))
        {
            logger.LogError("Invalid date range: {Range}. No matching rewards found.", request.Range);

            return ResultT<PagedResult<RewardsDTos>>.Failure(Error.Failure("400", "Invalid date range"));
        }

        var validationPagination =
            PaginationHelper.ValidatePagination<RewardsDTos>(request.PageNumber, request.PageSize, logger);

        if (!validationPagination.IsSuccess) return validationPagination.Error;

        if (DateRangeFuncs.TryGetValue(request.Range, out var dateRangeFunc))
        {
            var result = await decoratedCache.GetOrCreateAsync(
                $"rewards-get-date-range-{request.Range}-page-{request.PageNumber}-size-{request.PageSize}",
                async () =>
                {
                    var rewardsEnumerable = await dateRangeFunc(rewardsRepository, request.PageNumber, request.PageSize,
                        cancellationToken);

                    var items = rewardsEnumerable.Items ?? [];

                    var rewardsDTos = items.Select(RewardsMapper.ToDto);

                    PagedResult<RewardsDTos> resultPaged = new()
                    {
                        TotalItems = rewardsEnumerable.TotalItems,
                        TotalPages = rewardsEnumerable.TotalPages,
                        CurrentPage = rewardsEnumerable.CurrentPage,
                        Items = rewardsDTos
                    };

                    return resultPaged;
                },
                cancellationToken: cancellationToken);

            var rewardsDTosEnumerable = result.Items ?? [];
            if (!rewardsDTosEnumerable.Any())
            {
                logger.LogError("No rewards found for the given date range: {Range}", request.Range);

                return ResultT<PagedResult<RewardsDTos>>.Failure(Error.Failure("400", "The list is empty"));
            }

            logger.LogInformation("Successfully retrieved {Count} rewards for date range: {Range}",
                rewardsDTosEnumerable.Count(),
                request.Range);

            return ResultT<PagedResult<RewardsDTos>>.Success(result);
        }

        logger.LogError("Invalid date range: {Range}. No matching rewards found.", request.Range);

        return ResultT<PagedResult<RewardsDTos>>.Failure(Error.Failure("400", "Invalid date range"));
    }

    #region Private Methods

    private static readonly
        Dictionary<DateRangeType, Func<IRewardsRepository, int, int, CancellationToken,
            Task<PagedResult<Domain.Models.Rewards>>>> DateRangeFuncs =
            new()
            {
                {
                    DateRangeType.Today,
                    async (repo, pageNumber, pageSize, cancellationToken) =>
                        await repo.GetRewardsByDateRangeAsync(
                            DateTime.UtcNow.Date,
                            DateTime.UtcNow.AddDays(1).AddTicks(-1),
                            pageNumber,
                            pageSize,
                            cancellationToken
                        )
                },
                {
                    DateRangeType.Week,
                    async (repo, pageNumber, pageSize, cancellationToken) =>
                        await repo.GetRewardsByDateRangeAsync(
                            DateTime.UtcNow.Date.AddDays(-7),
                            DateTime.UtcNow.Date.AddTicks(-7),
                            pageNumber,
                            pageSize,
                            cancellationToken
                        )
                },
                {
                    DateRangeType.Month,
                    async (repo, pageNumber, pageSize, cancellationToken) =>
                        await repo.GetRewardsByDateRangeAsync(
                            new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1),
                            new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(1).AddTicks(-1),
                            pageNumber,
                            pageSize,
                            cancellationToken
                        )
                },
                {
                    DateRangeType.Year,
                    async (repo, pageNumber, pageSize, cancellationToken) =>
                        await repo.GetRewardsByDateRangeAsync(
                            new DateTime(DateTime.UtcNow.Year, 1, 1),
                            new DateTime(DateTime.UtcNow.Year + 1, 1, 1).AddTicks(-1),
                            pageNumber,
                            pageSize,
                            cancellationToken
                        )
                },
            };

    #endregion
}