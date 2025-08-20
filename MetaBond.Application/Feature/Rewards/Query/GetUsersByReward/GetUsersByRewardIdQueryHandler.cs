using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.DTOs.Rewards;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using MetaBond.Domain.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Rewards.Query.GetUsersByReward;

internal sealed class GetUsersByRewardIdQueryHandler(
    IRewardsRepository rewardsRepository,
    ILogger<GetUsersByRewardIdQueryHandler> logger,
    IDistributedCache decorated
) :
    IQueryHandler<GetUsersByRewardIdQuery,
        IEnumerable<RewardsWithUserDTos>>
{
    public async Task<ResultT<IEnumerable<RewardsWithUserDTos>>> Handle(
        GetUsersByRewardIdQuery request,
        CancellationToken cancellationToken)
    {
        var reward = await rewardsRepository.GetByIdAsync(request.RewardsId);
        if (reward is null)
        {
            logger.LogWarning("Reward with ID {RewardId} not found.", request.RewardsId);

            return ResultT<IEnumerable<RewardsWithUserDTos>>.Failure(
                Error.NotFound("404", $"Reward with ID {request.RewardsId} not found."));
        }

        var rewardsWithUser = await decorated.GetOrCreateAsync(
            $"Get-User-By-Rewards-By-Id-{request.RewardsId}",
            async () =>
            {
                var rewards = await rewardsRepository.GetUsersByRewardIdAsync(request.RewardsId, cancellationToken);

                IEnumerable<RewardsWithUserDTos> rewardsWithUserDTos = rewards.Select(RewardsMapper.RewardsWithUserToDto);

                return rewardsWithUserDTos;
            },
            cancellationToken: cancellationToken);

        var rewardsWithUserDTosEnumerable = rewardsWithUser.ToList();
        if (!rewardsWithUserDTosEnumerable.Any())
        {
            logger.LogWarning("No users associated with reward ID {RewardId}.", request.RewardsId);

            return ResultT<IEnumerable<RewardsWithUserDTos>>.Failure(
                Error.NotFound("404", $"No users associated with reward ID {request.RewardsId}."));
        }

        logger.LogInformation("Retrieved {Count} users associated with reward ID {RewardId}.",
            rewardsWithUserDTosEnumerable.Count(), request.RewardsId);

        return ResultT<IEnumerable<RewardsWithUserDTos>>.Success(rewardsWithUserDTosEnumerable);
    }
}