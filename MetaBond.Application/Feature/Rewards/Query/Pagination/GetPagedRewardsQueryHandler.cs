using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Rewards;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Rewards.Query.Pagination;

internal sealed class GetPagedRewardsQueryHandler(
    IRewardsRepository rewardsRepository,
    IDistributedCache decoratedCache,
    ILogger<GetPagedRewardsQueryHandler> logger)
    : IQueryHandler<GetPagedRewardsQuery, PagedResult<RewardsDTos>>
{
    public async Task<ResultT<PagedResult<RewardsDTos>>> Handle(
        GetPagedRewardsQuery request,
        CancellationToken cancellationToken)
    {
        if (request != null)
        {
            if (request.PageNumber <= 0 || request.PageSize <= 0)
            {
                logger.LogWarning(
                    "Invalid pagination parameters: PageNumber={PageNumber}, PageSize={PageSize}. Both must be greater than zero.",
                    request.PageNumber, request.PageSize);

                return ResultT<PagedResult<RewardsDTos>>.Failure(Error.Failure("400",
                    "Page Number and Page Size must be greater than zero."));
            }

            var pagedRewards = await decoratedCache.GetOrCreateAsync(
                $"get-paged-rewards-{request.PageNumber}-size-{request.PageSize}",
                async () =>
                {
                    var rewards = await rewardsRepository.GetPagedRewardsAsync(
                        request.PageNumber,
                        request.PageSize,
                        cancellationToken);

                    var rewardsDto = rewards.Items!.Select(RewardsMapper.ToDto);

                    PagedResult<RewardsDTos> resultPaged = new()
                    {
                        TotalItems = rewards.TotalItems,
                        TotalPages = rewards.TotalPages,
                        CurrentPage = rewards.CurrentPage,
                        Items = rewardsDto
                    };

                    return resultPaged;
                },
                cancellationToken: cancellationToken);

            if (!pagedRewards.Items!.Any())
            {
                logger.LogWarning("No rewards found for the requested page {PageNumber}.", request.PageNumber);

                return ResultT<PagedResult<RewardsDTos>>.Failure(Error.Failure("400",
                    "No rewards available for the requested page."));
            }

            var rewardsDTosEnumerable = pagedRewards.Items.ToList();

            logger.LogInformation("Successfully retrieved {Count} rewards for page {PageNumber}.",
                rewardsDTosEnumerable.Count(), request.PageNumber);

            return ResultT<PagedResult<RewardsDTos>>.Success(pagedRewards);
        }

        logger.LogError("Received a null request for paginated rewards.");

        return ResultT<PagedResult<RewardsDTos>>.Failure(Error.Failure("400", "Invalid request."));
    }
}