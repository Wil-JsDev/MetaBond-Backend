using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Helpers;
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
        var friendship =
            await EntityHelper.GetEntityByIdAsync(friendshipRepository.GetByIdAsync,
                request.Id,
                "Friendship",
                logger);

        if (!friendship.IsSuccess) return ResultT<FriendshipDTos>.Failure(friendship.Error!);

        var friendshipDTos = FriendshipMapper.MapFriendshipDTos(friendship.Value);

        logger.LogInformation("Friendship retrieved successfully. ID: {FriendshipId}, Status: {Status}",
            friendship.Value.Id, friendship.Value.Status);

        return ResultT<FriendshipDTos>.Success(friendshipDTos);
    }
}