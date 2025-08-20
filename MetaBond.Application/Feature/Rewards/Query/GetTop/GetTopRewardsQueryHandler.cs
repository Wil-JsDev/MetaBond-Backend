using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Rewards;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Rewards.Query.GetTop;

internal sealed class GetTopRewardsQueryHandler(
    IRewardsRepository rewardsRepository,
    IDistributedCache decoratedCache,
    ILogger<GetTopRewardsQueryHandler> logger)
    : IQueryHandler<GetTopRewardsQuery, IEnumerable<RewardsWithUserDTos>>
{
    public async Task<ResultT<IEnumerable<RewardsWithUserDTos>>> Handle(
        GetTopRewardsQuery request,
        CancellationToken cancellationToken)
    {
        var result = await decoratedCache.GetOrCreateAsync(
            $"rewards-get-top-by-points-{request.TopCount}",
            async () =>
            {
                var rewards = await rewardsRepository.GetTopRewardsByPointsAsync(request.TopCount, cancellationToken);

                return rewards.Select(RewardsMapper.RewardsWithUserToDto);
            },
            cancellationToken: cancellationToken);

        var rewardsDTosEnumerable = result.ToList();
        if (!rewardsDTosEnumerable.Any())
        {
            logger.LogError("No top rewards found");

            return ResultT<IEnumerable<RewardsWithUserDTos>>.Failure(Error.Failure("400", "The list is empty"));
        }

        logger.LogInformation("Successfully retrieved {Count} top rewards.", rewardsDTosEnumerable.Count());

        return ResultT<IEnumerable<RewardsWithUserDTos>>.Success(rewardsDTosEnumerable);
    }
}