using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Friendship.Query.GetCountByStatus;

internal sealed class GetCountByStatusFriendshipQueryHandler(
    IFriendshipRepository friendshipRepository,
    IDistributedCache decoratedCache,
    ILogger<GetCountByStatusFriendshipQueryHandler> logger)
    : IQueryHandler<GetCountByStatusFriendshipQuery, int>
{
    public async Task<ResultT<int>> Handle(
        GetCountByStatusFriendshipQuery request,
        CancellationToken cancellationToken)
    {
        var friendship = CountStatusAsync();
        if (friendship.TryGetValue((request.Status), out var countFriendship))
        {
            string cacheKey = $"GetCountByStatusFriendshipQuery-{request.Status}";
            var result = await decoratedCache.GetOrCreateAsync(
                cacheKey,
                async () =>
                {
                    var count = await countFriendship(cancellationToken);
                    return count;
                },
                cancellationToken: cancellationToken);

            logger.LogInformation("Successfully retrieved count for status {Status}: {Count}", request.Status, result);

            return ResultT<int>.Success(result);
        }

        logger.LogError("Failed to retrieve count: Invalid status {Status}", request.Status);

        return ResultT<int>.Failure(Error.Failure("400", "Invalid status"));
    }

    #region Private Methods

    private Dictionary<Status, Func<CancellationToken, Task<int>>> CountStatusAsync()
    {
        return new Dictionary<Status, Func<CancellationToken, Task<int>>>
        {
            {
                (Status.Accepted),
                async cancellationToken =>
                    await friendshipRepository.CountByStatusAsync(Status.Accepted, cancellationToken)
            },
            {
                (Status.Pending),
                async cancellationToken =>
                    await friendshipRepository.CountByStatusAsync(Status.Pending, cancellationToken)
            },
            {
                (Status.Blocked),
                async cancellationToken =>
                    await friendshipRepository.CountByStatusAsync(Status.Blocked, cancellationToken)
            }
        };
    }

    #endregion
}