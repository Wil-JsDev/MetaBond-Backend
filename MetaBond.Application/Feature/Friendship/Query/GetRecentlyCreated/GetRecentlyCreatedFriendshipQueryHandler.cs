using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Friendship.Query.GetRecentlyCreated;

internal sealed class GetRecentlyCreatedFriendshipQueryHandler(
    IFriendshipRepository friendshipRepository,
    IDistributedCache decoratedCache,
    ILogger<GetRecentlyCreatedFriendshipQueryHandler> logger)
    : IQueryHandler<GetRecentlyCreatedFriendshipQuery, IEnumerable<FriendshipDTos>>
{
    public async Task<ResultT<IEnumerable<FriendshipDTos>>> Handle(
        GetRecentlyCreatedFriendshipQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Limit <= 0)
        {
            logger.LogError("Invalid limit: {Limit}", request.Limit);

            return ResultT<IEnumerable<FriendshipDTos>>.Failure(
                Error.Failure("400", "Invalid limit. Limit must be greater than zero."));
        }

        string cacheKey = $"GetRecentlyCreatedFriendship-{request.Limit}";
        var friendshipRecently = await decoratedCache.GetOrCreateAsync(
            cacheKey,
            async () =>
            {
                var recentFriendship =
                    await friendshipRepository.GetRecentlyCreatedAsync(request.Limit, cancellationToken);

                var friendshipDTos = recentFriendship.Select(FriendshipMapper.MapFriendshipDTos).ToList();

                return friendshipDTos;
            },
            cancellationToken: cancellationToken);

        if (!friendshipRecently.Any())
        {
            logger.LogError("No recent friendships found for the given limit: {Limit}", request.Limit);

            return ResultT<IEnumerable<FriendshipDTos>>.Failure(
                Error.Failure("400", "No recent friendships found."));
        }


        IEnumerable<FriendshipDTos> value = friendshipRecently.ToList();
        logger.LogInformation("Successfully retrieved {Count} recent friendships with limit: {Limit}",
            value.Count(), request.Limit);

        return ResultT<IEnumerable<FriendshipDTos>>.Success(value);
    }
}