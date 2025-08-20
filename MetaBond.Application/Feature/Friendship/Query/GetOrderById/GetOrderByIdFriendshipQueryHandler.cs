using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Friendship.Query.GetOrderById;

internal sealed class GetOrderByIdFriendshipQueryHandler(
    IFriendshipRepository friendshipRepository,
    IDistributedCache decoratedCache,
    ILogger<GetOrderByIdFriendshipQueryHandler> logger)
    : IQueryHandler<GetOrderByIdFriendshipQuery, IEnumerable<FriendshipDTos>>
{
    public async Task<ResultT<IEnumerable<FriendshipDTos>>> Handle(
        GetOrderByIdFriendshipQuery request,
        CancellationToken cancellationToken)
    {
        var friendshipSort = GetSort();
        if (friendshipSort.TryGetValue((request.Sort!.ToUpper()), out var getSortFriendship))
        {
            string cacheKey = $"GetOrderByIdFriendshipQuery-{request.Sort}";
            var friendshipList = await decoratedCache.GetOrCreateAsync(
                cacheKey,
                async () =>
                {
                    var sortFriendship = await getSortFriendship(cancellationToken);

                    IEnumerable<FriendshipDTos> friendshipDTos = sortFriendship.Select(FriendshipMapper.MapFriendshipDTos);
                    
                    return friendshipDTos;
                },
                cancellationToken: cancellationToken);

            var friendshipDTosEnumerable = friendshipList.ToList();
            if (!friendshipDTosEnumerable.Any())
            {
                logger.LogError("No friendships found for the sorted order: {Sort}", request.Sort);

                return ResultT<IEnumerable<FriendshipDTos>>.Failure(Error.Failure("400", "The list is empty"));
            }

            logger.LogInformation("Successfully retrieved {Count} friendships sorted by: {Sort}",
                friendshipDTosEnumerable.Count(), request.Sort);

            return ResultT<IEnumerable<FriendshipDTos>>.Success(friendshipDTosEnumerable);
        }

        logger.LogError("Invalid order parameter: {Sort}. Expected 'asc' or 'desc'.", request.Sort);

        return ResultT<IEnumerable<FriendshipDTos>>.Failure
            (Error.Failure("400", "Invalid order parameter. Please specify 'asc' or 'desc'."));
    }

    #region Private Methods

    private Dictionary<string, Func<CancellationToken, Task<IEnumerable<Domain.Models.Friendship>>>> GetSort()
    {
        return new Dictionary<string, Func<CancellationToken, Task<IEnumerable<Domain.Models.Friendship>>>>
        {
            { "asc", async cancellationToken => await friendshipRepository.OrderByIdAscAsync(cancellationToken) },
            { "desc", async cancellationToken => await friendshipRepository.OrderByIdDescAsync(cancellationToken) }
        };
    }

    #endregion
}