using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Friendship.Query.GetById;

internal sealed class GetByIdFriendshipQueryHandler(
    IFriendshipRepository friendshipRepository,
    IDistributedCache decoratedCache,
    ILogger<GetByIdFriendshipQueryHandler> logger)
    : IQueryHandler<GetByIdFriendshipQuery, FriendshipDTos>
{
    public async Task<ResultT<FriendshipDTos>> Handle(
        GetByIdFriendshipQuery request, 
        CancellationToken cancellationToken)
    {
        var friendship = await decoratedCache.GetOrCreateAsync(
            $"friendship-{request.Id}", 
            async () => await friendshipRepository.GetByIdAsync(request.Id), 
            cancellationToken: cancellationToken);
        
        if (friendship != null)
        {
            FriendshipDTos friendshipDTos = new
            ( 
                FriendshipId: friendship.Id,
                Status: friendship.Status,
                CreatedAt: friendship.CreateAdt
            );

            logger.LogInformation("Friendship retrieved successfully. ID: {FriendshipId}, Status: {Status}",
                friendship.Id, friendship.Status);

            return ResultT<FriendshipDTos>.Success(friendshipDTos);

        }
        logger.LogError("Failed to retrieve friendship: ID {FriendshipId} not found.", request.Id);

        return ResultT<FriendshipDTos>.Failure(Error.NotFound("404", $"{request.Id} not found"));
    }
}