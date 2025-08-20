using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
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
        var friendship = await friendshipRepository.GetByIdAsync(request.Id);
        if (friendship != null)
        {
            friendship.Status = request.Status;

            await friendshipRepository.UpdateAsync(friendship, cancellationToken);

            logger.LogInformation("Friendship with ID: {FriendshipId} updated successfully. New Status: {Status}",
                friendship.Id, friendship.Status);

            UpdateFriendshipDTos friendshipDTos = new
            (
                StatusFriendship: friendship.Status
            );

            return ResultT<UpdateFriendshipDTos>.Success(friendshipDTos);
        }

        logger.LogError("Failed to update friendship: ID {FriendshipId} not found.", request.Id);

        return ResultT<UpdateFriendshipDTos>.Failure(Error.NotFound("404", $"{request.Id} not found"));
    }
}