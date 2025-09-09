using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Rewards;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Rewards.Query.GetRecent;

internal sealed class GetMostRecentRewardsQueryHandler(
    IRewardsRepository rewardsRepository,
    IDistributedCache decoratedCache,
    ILogger<GetMostRecentRewardsQueryHandler> logger)
    : IQueryHandler<GetMostRecentRewardsQuery, RewardsDTos>
{
    public async Task<ResultT<RewardsDTos>> Handle(
        GetMostRecentRewardsQuery request,
        CancellationToken cancellationToken)
    {
        var rewardDto = await decoratedCache.GetOrCreateAsync(
            "rewards-get-most-recent",
            async () =>
            {
                var reward = await rewardsRepository.GetMostRecentRewardAsync(cancellationToken);
                return RewardsMapper.ToDto(reward);
            },
            cancellationToken: cancellationToken);

        if (rewardDto is null)
        {
            logger.LogWarning("No recent rewards found.");
            return ResultT<RewardsDTos>.Failure(Error.Failure("400", "No rewards available"));
        }

        logger.LogInformation("Most recent reward found with ID: {RewardsId}", rewardDto.RewardsId);

        return ResultT<RewardsDTos>.Success(rewardDto);
    }
}