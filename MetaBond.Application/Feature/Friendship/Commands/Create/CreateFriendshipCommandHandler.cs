using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Friendship.Commands.Create;

internal sealed class CreateFriendshipCommandHandler(
    IFriendshipRepository friendshipRepository,
    ILogger<CreateFriendshipCommandHandler> logger)
    : ICommandHandler<CreateFriendshipCommand, FriendshipDTos>
{
    public async Task<ResultT<FriendshipDTos>> Handle(
        CreateFriendshipCommand request, 
        CancellationToken cancellationToken)
    {
        if (request != null)
        {
            Domain.Models.Friendship friendship = new()
            {
                Id = Guid.NewGuid(),
                Status = request.Status
            };

            await friendshipRepository.CreateAsync(friendship, cancellationToken);

            logger.LogInformation("Friendship created successfully with ID: {FriendshipId}", friendship.Id);

            FriendshipDTos friendshipDTos = new
            ( 
                FriendshipId: friendship.Id,
                Status: friendship.Status,
                CreatedAt: friendship.CreateAdt
            );

            return ResultT<FriendshipDTos>.Success(friendshipDTos);

        }
        logger.LogError("Failed to create friendship: request is null.");

        return ResultT<FriendshipDTos>.Failure(Error.Failure("400", "Failed to create the friendship"));
    }
}