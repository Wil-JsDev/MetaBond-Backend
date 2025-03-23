using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Friendship.Commands.Update;

internal sealed class UpdateFriendshipCommandHandler(
    IFriendshipRepository friendshipRepository,
    ILogger<UpdateFriendshipCommandHandler> logger)
    : ICommandHandler<UpdateFriendshipCommand, FriendshipDTos>
{
    public async Task<ResultT<FriendshipDTos>> Handle(
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

            FriendshipDTos friendshipDTos = new
            (
                FriendshipId: friendship.Id,
                Status: friendship.Status,
                CreatedAt: friendship.CreateAdt
            );

            return ResultT<FriendshipDTos>.Success(friendshipDTos);

        }

        logger.LogError("Failed to update friendship: ID {FriendshipId} not found.", request.Id);

        return ResultT<FriendshipDTos>.Failure(Error.NotFound("404",$"{request.Id} not found")); 
            
    }
}