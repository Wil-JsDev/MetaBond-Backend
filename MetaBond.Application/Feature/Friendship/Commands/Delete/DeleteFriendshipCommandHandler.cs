using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.Helpers;
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
        var friendship =
            await EntityHelper.GetEntityByIdAsync(friendshipRepository.GetByIdAsync,
                request.Id,
                "Friendship",
                logger);

        if (!friendship.IsSuccess) return ResultT<Guid>.Failure(friendship.Error!);

        await friendshipRepository.DeleteAsync(friendship.Value, cancellationToken);

        logger.LogInformation("Friendship with ID: {FriendshipId} deleted successfully.", friendship.Value.Id);

        return ResultT<Guid>.Success(friendship.Value.Id);
    }
}