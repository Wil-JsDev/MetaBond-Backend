using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Friendship.Query.GetCreated.GetCreatedAfter;

internal sealed class GetCreatedAfterFriendshipQueryHandler(
    IFriendshipRepository friendshipRepository,
    IDistributedCache decoratedCache,
    ILogger<GetCreatedAfterFriendshipQueryHandler> logger)
    : IQueryHandler<GetCreatedAfterFriendshipQuery, IEnumerable<FriendshipDTos>>
{
    public async Task<ResultT<IEnumerable<FriendshipDTos>>> Handle(
        GetCreatedAfterFriendshipQuery request,
        CancellationToken cancellationToken)
    {
        var friendshipAfter = GetCreatedAfterFriendship();
        if (friendshipAfter.TryGetValue((request.DateRange), out var createdAfter))
        {
            string cacheKey = $"GetCreatedAfterFriendshipQuery-{request.DateRange}";
            var result = await decoratedCache.GetOrCreateAsync(
                cacheKey,
                async () =>
                {
                    var friendshipList = await createdAfter(cancellationToken);

                    var friendshipDTos = friendshipList.Select(FriendshipMapper.MapFriendshipDTos);
                    return friendshipDTos;
                },
                cancellationToken: cancellationToken);

            var dTosEnumerable = result.ToList();
            if (!dTosEnumerable.Any())
            {
                logger.LogError("No friendships found after the specified date range: {DateRange}", request.DateRange);

                return ResultT<IEnumerable<FriendshipDTos>>.Failure(Error.Failure("400", "The list is empty"));
            }


            IEnumerable<FriendshipDTos> friendshipDTosEnumerable = dTosEnumerable.ToList();
            logger.LogInformation("Retrieved {Count} friendships after the date range: {DateRange}",
                friendshipDTosEnumerable.Count(), request.DateRange);

            return ResultT<IEnumerable<FriendshipDTos>>.Success(friendshipDTosEnumerable);
        }

        logger.LogError("Failed to retrieve friendships: Invalid date range type {DateRange}", request.DateRange);

        return ResultT<IEnumerable<FriendshipDTos>>.Failure(Error.Failure("400", "Invalid status"));
    }

    private Dictionary<DateRangeType, Func<CancellationToken, Task<IEnumerable<Domain.Models.Friendship>>>>
        GetCreatedAfterFriendship()
    {
        return new Dictionary<DateRangeType, Func<CancellationToken, Task<IEnumerable<Domain.Models.Friendship>>>>()
        {
            {
                DateRangeType.Today,
                async cancellationToken =>
                    await friendshipRepository.GetCreatedAfterAsync(DateTime.UtcNow.Date, cancellationToken)
            },
            {
                DateRangeType.Week,
                async cancellationToken =>
                    await friendshipRepository.GetCreatedAfterAsync(DateTime.UtcNow.AddDays(-7), cancellationToken)
            },
            {
                DateRangeType.Month,
                async cancellationToken =>
                    await friendshipRepository.GetCreatedAfterAsync(DateTime.UtcNow.AddMonths(-1), cancellationToken)
            },
            {
                DateRangeType.Year,
                async cancellationToken =>
                    await friendshipRepository.GetCreatedAfterAsync(DateTime.UtcNow.AddYears(-1), cancellationToken)
            }
        };
    }
}