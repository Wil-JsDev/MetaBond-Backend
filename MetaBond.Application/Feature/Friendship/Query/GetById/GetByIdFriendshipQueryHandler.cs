using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Friendship.Query.GetById;

internal sealed class GetByIdFriendshipQueryHandler(
    IFriendshipRepository friendshipRepository,
    ILogger<GetByIdFriendshipQueryHandler> logger)
    : IQueryHandler<GetByIdFriendshipQuery, FriendshipDTos>
{
    public async Task<ResultT<FriendshipDTos>> Handle(
        GetByIdFriendshipQuery request,
        CancellationToken cancellationToken)
    {
        var friendship = await friendshipRepository.GetByIdAsync(request.Id);
        if (friendship is not null)
        {
            var friendshipDTos = FriendshipMapper.MapFriendshipDTos(friendship);

            logger.LogInformation("Friendship retrieved successfully. ID: {FriendshipId}, Status: {Status}",
                friendship.Id, friendship.Status);

            return ResultT<FriendshipDTos>.Success(friendshipDTos);
        }

        logger.LogError("Failed to retrieve friendship: ID {FriendshipId} not found.", request.Id);

        return ResultT<FriendshipDTos>.Failure(Error.NotFound("404", $"{request.Id} not found"));
    }
}