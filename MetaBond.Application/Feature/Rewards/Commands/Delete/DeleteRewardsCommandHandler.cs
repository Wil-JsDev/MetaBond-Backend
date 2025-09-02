﻿using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.Helpers;
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
        var reward = await EntityHelper.GetEntityByIdAsync(
            repository.GetByIdAsync,
            request.RewardsId,
            "Rewards",
            logger
        );

        if (reward.IsSuccess)
        {
            await repository.DeleteAsync(reward.Value, cancellationToken);

            logger.LogInformation("Reward with ID {RewardsId} deleted successfully", reward.Value.Id);

            return ResultT<Guid>.Success(reward.Value.Id);
        }

        logger.LogError("Reward with ID {RewardsId} not found", request.RewardsId);

        return ResultT<Guid>.Failure(Error.Failure("404", "Reward not found"));
    }
}