using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Friendship.Query.GetFriendshipWithUser;

internal sealed class GetFriendshipWithUsersQueryHandler(
    IFriendshipRepository friendshipRepository,
    ILogger<GetFriendshipWithUsersQueryHandler> logger,
    IDistributedCache decoratedCache)
    : IQueryHandler<GetFriendshipWithUsersQuery, IEnumerable<FriendshipWithUserDTos>>
{
    public async Task<ResultT<IEnumerable<FriendshipWithUserDTos>>> Handle(
        GetFriendshipWithUsersQuery request,
        CancellationToken cancellationToken)
    {
        var friendship =
            await EntityHelper.GetEntityByIdAsync(friendshipRepository.GetByIdAsync,
                request.FriendshipId ?? Guid.Empty,
                "Friendship",
                logger);

        if (!friendship.IsSuccess) return ResultT<IEnumerable<FriendshipWithUserDTos>>.Failure(friendship.Error!);

        var getFriendshipWithUser =
            await decoratedCache.GetOrCreateAsync($"GetFriendshipWithUsersQueryCache-{request.FriendshipId}",
                async () =>
                {
                    var friendshipUser = await friendshipRepository.GetFriendshipWithUsersAsync(
                        (Guid)request.FriendshipId!,
                        cancellationToken);

                    return friendshipUser.ToFriendshipWithUserDtos().ToList();
                },
                cancellationToken: cancellationToken);

        if (!getFriendshipWithUser.Any())
        {
            logger.LogWarning("No friendship with users found for ID: {FriendshipId}", request.FriendshipId);

            return ResultT<IEnumerable<FriendshipWithUserDTos>>.Failure(Error.Failure("400",
                "No friendship with users found for the given ID."));
        }

        logger.LogInformation("Friendship with users loaded successfully for ID: {FriendshipId}", request.FriendshipId);

        return ResultT<IEnumerable<FriendshipWithUserDTos>>.Success(getFriendshipWithUser);
    }
}