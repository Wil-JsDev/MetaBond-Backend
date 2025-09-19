using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Friendship.Commands.Update;

internal sealed class UpdateFriendshipCommandHandler(
    IFriendshipRepository friendshipRepository,
    ILogger<UpdateFriendshipCommandHandler> logger)
    : ICommandHandler<UpdateFriendshipCommand, UpdateFriendshipDTos>
{
    public async Task<ResultT<UpdateFriendshipDTos>> Handle(
        UpdateFriendshipCommand request,
        CancellationToken cancellationToken)
    {
        var friendship =
            await EntityHelper.GetEntityByIdAsync(friendshipRepository.GetByIdAsync,
                request.Id,
                "Friendship",
                logger);

        if (!friendship.IsSuccess) return ResultT<UpdateFriendshipDTos>.Failure(friendship.Error!);

        friendship.Value.Status = request.Status;

        await friendshipRepository.UpdateAsync(friendship.Value, cancellationToken);

        logger.LogInformation("Friendship with ID: {FriendshipId} updated successfully. New Status: {Status}",
            friendship.Value.Id, friendship.Value.Status);

        UpdateFriendshipDTos friendshipDTos = new
        (
            StatusFriendship: friendship.Value.Status
        );

        return ResultT<UpdateFriendshipDTos>.Success(friendshipDTos);
    }
}