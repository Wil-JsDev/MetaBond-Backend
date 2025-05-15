using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Rewards;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Rewards.Query.GetTop;

internal sealed class GetTopRewardsQueryHandler(
    IRewardsRepository rewardsRepository,
    IDistributedCache decoratedCache,
    ILogger<GetTopRewardsQueryHandler> logger)
    : IQueryHandler<GetTopRewardsQuery, IEnumerable<RewardsDTos>>
{
    public async Task<ResultT<IEnumerable<RewardsDTos>>> Handle(
        GetTopRewardsQuery request, 
        CancellationToken cancellationToken)
    {

        if (request != null)
        {
            var rewardsList = await decoratedCache.GetOrCreateAsync(
                $"rewards-get-top-by-points-{request.TopCount}",
                async () =>
                    await rewardsRepository.GetTopRewardsByPointsAsync(request.TopCount, cancellationToken), 
                cancellationToken: cancellationToken);
            
            IEnumerable<Domain.Models.Rewards> rewardsEnumerable = rewardsList.ToList();
            if (!rewardsEnumerable.Any())
            {
                logger.LogError("No top rewards found");

                return ResultT<IEnumerable<RewardsDTos>>.Failure(Error.Failure("400","The list is empty"));
            }

            IEnumerable<RewardsDTos> rewardsDTos = rewardsEnumerable.Select(x => new RewardsDTos
            (
                RewardsId: x.Id,
                UserId: x.UserId,
                Description: x.Description,
                PointAwarded: x.PointAwarded,
                DateAwarded:  x.DateAwarded
            ));

            logger.LogInformation("Successfully retrieved {Count} top rewards.", rewardsEnumerable.Count());

            return ResultT<IEnumerable<RewardsDTos>>.Success(rewardsDTos);
        }
        logger.LogError("Received a null request for fetching top rewards.");

        return ResultT<IEnumerable<RewardsDTos>>.Failure(Error.Failure("400","Invalid request"));
    }
}