using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Friendship.Commands.Delete;

internal sealed class DeleteFriendshipCommandHandler(
    IFriendshipRepository friendshipRepository,
    ILogger<DeleteFriendshipCommandHandler> logger)
    : ICommandHandler<DeleteFriendshipCommand, Guid>
{
    public async Task<ResultT<Guid>> Handle(
        DeleteFriendshipCommand request, 
        CancellationToken cancellationToken)
    {
        var friendship = await friendshipRepository.GetByIdAsync(request.Id);
        if (friendship != null)
        {
            await friendshipRepository.DeleteAsync(friendship,cancellationToken);

            logger.LogInformation("Friendship with ID: {FriendshipId} deleted successfully.", friendship.Id);

            return ResultT<Guid>.Success(friendship.Id);
        }

        logger.LogError("Failed to delete friendship: ID {FriendshipId} not found.", request.Id);

        return ResultT<Guid>.Failure(Error.Failure("404", $"{request.Id} not found"));

    }
}