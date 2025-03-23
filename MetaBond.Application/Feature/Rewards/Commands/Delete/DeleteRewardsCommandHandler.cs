using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Rewards.Commands.Delete;

internal sealed class DeleteRewardsCommandHandler(
    IRewardsRepository repository,
    ILogger<DeleteRewardsCommandHandler> logger)
    : ICommandHandler<DeleteRewardsCommand, Guid>
{
    public async Task<ResultT<Guid>> Handle(
        DeleteRewardsCommand request, 
        CancellationToken cancellationToken)
    {
        var reward = await repository.GetByIdAsync(request.RewardsId);
        if (reward != null)
        {
            await repository.DeleteAsync(reward,cancellationToken);

            logger.LogInformation("Reward with ID {RewardsId} deleted successfully", reward.Id);

            return ResultT<Guid>.Success(reward.Id);
        }

        logger.LogError("Reward with ID {RewardsId} not found", request.RewardsId);

        return ResultT<Guid>.Failure(Error.Failure("404", "Reward not found"));
    }
}