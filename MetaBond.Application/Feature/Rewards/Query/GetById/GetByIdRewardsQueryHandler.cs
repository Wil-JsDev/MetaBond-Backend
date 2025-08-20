using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Rewards;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Rewards.Query.GetById;

internal sealed class GetByIdRewardsQueryHandler(
    IRewardsRepository rewardsRepository,
    IDistributedCache decoratedCache,
    ILogger<GetByIdRewardsQueryHandler> logger)
    : IQueryHandler<GetByIdRewardsQuery, RewardsDTos>
{
    public async Task<ResultT<RewardsDTos>> Handle(
        GetByIdRewardsQuery request,
        CancellationToken cancellationToken)
    {
        var reward = await rewardsRepository.GetByIdAsync(request.RewardsId);

        if (reward != null)
        {
            var rewardsDTos = RewardsMapper.ToDto(reward);

            logger.LogInformation("Reward with ID: {RewardsId} found.", request.RewardsId);

            return ResultT<RewardsDTos>.Success(rewardsDTos);
        }

        logger.LogWarning("Reward with ID: {RewardsId} not found.", request.RewardsId);

        return ResultT<RewardsDTos>.Failure(Error.NotFound("400", $"Reward with ID {request.RewardsId} not found"));
    }
}