using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Interfaces.Service;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Rewards.Commands.Delete;

internal sealed class DeleteRewardsCommandHandler(
    IRewardsRepository repository,
    ILogger<DeleteRewardsCommandHandler> logger,
    ICurrentService currentService
)
    : ICommandHandler<DeleteRewardsCommand, Guid>
{
    public async Task<ResultT<Guid>> Handle(
        DeleteRewardsCommand request,
        CancellationToken cancellationToken)
    {
        var reward = await EntityHelper.GetEntityByIdAsync(
            repository.GetByIdAsync,
            request.RewardsId,
            "Rewards",
            logger
        );

        if (!reward.IsSuccess) return reward.Error!;

        if (reward.Value.UserId != currentService.CurrentId && !currentService.IsAdmin)
        {
            logger.LogWarning("User {UserId} is not authorized to delete reward with ID {RewardsId}",
                currentService.CurrentId,
                request.RewardsId);

            return ResultT<Guid>.Failure(Error.Conflict("409", "You are not authorized to delete this reward."));
        }

        await repository.DeleteAsync(reward.Value, cancellationToken);

        logger.LogInformation("Reward with ID {RewardsId} deleted successfully", reward.Value.Id);

        return ResultT<Guid>.Success(reward.Value.Id);
    }
}