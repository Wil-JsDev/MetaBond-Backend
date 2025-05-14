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

        if (request.RewardsId != null)
        {
            var reward = await rewardsRepository.GetByIdAsync(request.RewardsId);
            if (reward == null)
            {
                logger.LogWarning("Reward with ID {RewardId} not found.", request.RewardsId);

                return ResultT<IEnumerable<RewardsWithUserDTos>>.Failure(
                    Error.NotFound("404", $"Reward with ID {request.RewardsId} not found."));
            }

            var rewardsWithUser = await decorated.GetOrCreateAsync(
                $"Get-User-By-Rewards-By-Id-{request.RewardsId}",
                async () => await rewardsRepository.GetUsersByRewardIdAsync(request.RewardsId, cancellationToken),
                cancellationToken: cancellationToken);

            IEnumerable<Domain.Models.Rewards> rewardsEnumerable = rewardsWithUser.ToList();
            if (!rewardsEnumerable.Any())
            {
                logger.LogWarning("No users associated with reward ID {RewardId}.", request.RewardsId);

                return ResultT<IEnumerable<RewardsWithUserDTos>>.Failure(
                    Error.NotFound("404", $"No users associated with reward ID {request.RewardsId}."));
            }

            IEnumerable<RewardsWithUserDTos> rewardsWithUserDTos = rewardsEnumerable.Select(RewardsMapper.ToDto);

            var rewardsWithUserDTosEnumerable = rewardsWithUserDTos.ToList();
            
            logger.LogInformation("Retrieved {Count} users associated with reward ID {RewardId}.",
                rewardsWithUserDTosEnumerable.Count(), request.RewardsId);

            return ResultT<IEnumerable<RewardsWithUserDTos>>.Success(rewardsWithUserDTosEnumerable);
        }

        logger.LogWarning("Request received with null RewardsId.");
        return ResultT<IEnumerable<RewardsWithUserDTos>>.Failure(
            Error.Failure("404", "Reward ID must be provided."));
    }
}