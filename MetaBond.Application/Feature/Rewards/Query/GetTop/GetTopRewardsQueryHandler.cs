using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Rewards;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Rewards.Query.GetTop;

internal sealed class GetTopRewardsQueryHandler(
    IRewardsRepository rewardsRepository,
    IDistributedCache decoratedCache,
    ILogger<GetTopRewardsQueryHandler> logger)
    : IQueryHandler<GetTopRewardsQuery, PagedResult<RewardsWithUserDTos>>
{
    public async Task<ResultT<PagedResult<RewardsWithUserDTos>>> Handle(
        GetTopRewardsQuery request,
        CancellationToken cancellationToken)
    {
        var validationPagination =
            PaginationHelper.ValidatePagination<RewardsWithUserDTos>(request.PageNumber, request.PageSize, logger);

        if (!validationPagination.IsSuccess) return validationPagination.Error;

        var result = await decoratedCache.GetOrCreateAsync(
            $"rewards-get-top-by-points-page-{request.PageNumber}-size-{request.PageSize}",
            async () =>
            {
                var rewards =
                    await rewardsRepository.GetTopRewardsByPointsAsync(request.PageNumber, request.PageSize,
                        cancellationToken);

                var items = rewards.Items ?? [];
                var dtos = items.Select(RewardsMapper.RewardsWithUserToDto).ToList();
                PagedResult<RewardsWithUserDTos> pagedResult = new()
                {
                    TotalItems = rewards.TotalItems,
                    TotalPages = rewards.TotalPages,
                    CurrentPage = rewards.CurrentPage,
                    Items = dtos
                };

                return pagedResult;
            },
            cancellationToken: cancellationToken);

        var rewardsDTosEnumerable = result.Items?.ToList() ?? new List<RewardsWithUserDTos>();
        if (!rewardsDTosEnumerable.Any())
        {
            logger.LogError("No top rewards found. Page: {PageNumber}, Size: {PageSize}", request.PageNumber,
                request.PageSize);

            return ResultT<PagedResult<RewardsWithUserDTos>>.Failure(Error.Failure("400", "The list is empty"));
        }

        logger.LogInformation("Successfully retrieved {Count} top rewards. Page: {PageNumber}, Size: {PageSize}",
            rewardsDTosEnumerable.Count, request.PageNumber, request.PageSize);

        return ResultT<PagedResult<RewardsWithUserDTos>>.Success(result);
    }
}