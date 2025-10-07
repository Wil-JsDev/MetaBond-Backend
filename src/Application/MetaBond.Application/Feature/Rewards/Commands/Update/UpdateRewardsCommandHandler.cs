using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Rewards;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Interfaces.Service;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Rewards.Commands.Update;

internal sealed class UpdateRewardsCommandHandler(
    IRewardsRepository rewardsRepository,
    ILogger<UpdateRewardsCommandHandler> logger,
    ICurrentService currentService
)
    : ICommandHandler<UpdateRewardsCommand, RewardsDTos>
{
    public async Task<ResultT<RewardsDTos>> Handle(
        UpdateRewardsCommand request,
        CancellationToken cancellationToken)
    {
        var reward = await EntityHelper.GetEntityByIdAsync(
            rewardsRepository.GetByIdAsync,
            request.RewardsId,
            "Rewards",
            logger
        );
        if (!reward.IsSuccess) return reward.Error!;

        if (reward.Value.UserId != currentService.CurrentId && !currentService.IsAdmin)
        {
            return ResultT<RewardsDTos>.Failure(Error.Conflict("409", "You are not authorized to update this reward."));
        }

        reward.Value.Description = request.Description;

        reward.Value.PointAwarded = request.PointAwarded;

        await rewardsRepository.UpdateAsync(reward.Value, cancellationToken);

        logger.LogInformation("Reward with ID: {RewardsId} updated successfully.", request.RewardsId);

        var rewardsDTos = RewardsMapper.ToDto(reward.Value);

        return ResultT<RewardsDTos>.Success(rewardsDTos);
    }
}