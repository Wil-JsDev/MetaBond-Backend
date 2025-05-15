using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Rewards;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Rewards.Commands.Update;

internal sealed class UpdateRewardsCommandHandler(
    IRewardsRepository rewardsRepository,
    ILogger<UpdateRewardsCommandHandler> logger)
    : ICommandHandler<UpdateRewardsCommand, RewardsDTos>
{
    public async Task<ResultT<RewardsDTos>> Handle(
        UpdateRewardsCommand request, 
        CancellationToken cancellationToken)
    {
        var reward = await rewardsRepository.GetByIdAsync(request.RewardsId);
        if (reward != null)
        {
            reward.Description = request.Description;

            reward.PointAwarded = request.PointAwarded;

            await rewardsRepository.UpdateAsync(reward,cancellationToken);

            logger.LogInformation("Reward with ID: {RewardsId} updated successfully.", request.RewardsId);

            RewardsDTos rewardsDTos = new
            (
                RewardsId: reward.Id,
                UserId:  reward.UserId,
                Description: reward.Description,
                PointAwarded: reward.PointAwarded,
                DateAwarded: reward.DateAwarded
            );

            return ResultT<RewardsDTos>.Success(rewardsDTos);
        }

        logger.LogError("No reward found with ID: {RewardsId}", request.RewardsId);

        return ResultT<RewardsDTos>.Failure(Error.NotFound("404", $"{request.RewardsId} not found"));
    }
}