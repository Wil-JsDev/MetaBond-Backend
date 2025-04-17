using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Rewards;
using MetaBond.Application.Interfaces.Repository;
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
        var rewardRecent = await decoratedCache.GetOrCreateAsync(
            "rewards-get-most-recent",
            async () => await rewardsRepository.GetMostRecentRewardAsync(cancellationToken), 
            cancellationToken: cancellationToken);
        
        if (rewardRecent != null)
        {
            logger.LogInformation("Most recent reward found with ID: {RewardsId}", rewardRecent.Id);

            RewardsDTos rewardsDTos = new
            (
                RewardsId: rewardRecent.Id,
                Description: rewardRecent.Description,
                PointAwarded: rewardRecent.PointAwarded,
                DateAwarded: rewardRecent.DateAwarded
            );
                
            return ResultT<RewardsDTos>.Success(rewardsDTos);

        }
        logger.LogWarning("No recent rewards found.");

        return ResultT<RewardsDTos>.Failure(Error.Failure("400", "No rewards available"));
    }
}