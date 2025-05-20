using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Interfaces.Repository;
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
            var friendshipList = await decoratedCache.GetOrCreateAsync(
                cacheKey,
                async () => await createdAfter(cancellationToken), 
                cancellationToken: cancellationToken);

            IEnumerable<Domain.Models.Friendship> friendships = friendshipList.ToList();
            if (!friendships.Any())
            {
                logger.LogError("No friendships found after the specified date range: {DateRange}", request.DateRange);

                return ResultT<IEnumerable<FriendshipDTos>>.Failure(Error.Failure("400", "The list is empty"));
            }

            var friendshipDTos = friendships.Select(x => new FriendshipDTos
            (
                FriendshipId: x.Id,
                Status: x.Status,
                RequesterId: x.RequesterId,
                AddresseeId: x.AddresseeId,
                CreatedAt: x.CreateAdt
            ));

            IEnumerable<FriendshipDTos> friendshipDTosEnumerable = friendshipDTos.ToList();
            logger.LogInformation("Retrieved {Count} friendships after the date range: {DateRange}", 
                friendshipDTosEnumerable.Count(), request.DateRange);

            return ResultT<IEnumerable<FriendshipDTos>>.Success(friendshipDTosEnumerable);
        }
        logger.LogError("Failed to retrieve friendships: Invalid date range type {DateRange}", request.DateRange);

        return ResultT<IEnumerable<FriendshipDTos>>.Failure(Error.Failure("400", "Invalid status"));
    }

    private Dictionary<DateRangeType, Func<CancellationToken, Task<IEnumerable<Domain.Models.Friendship>>>> GetCreatedAfterFriendship()
    {
        return new Dictionary<DateRangeType, Func<CancellationToken, Task<IEnumerable<Domain.Models.Friendship>>>>()
        {
            { DateRangeType.Today, async cancellationToken => await friendshipRepository.GetCreatedAfterAsync(DateTime.UtcNow.Date, cancellationToken) },
            { DateRangeType.Week, async cancellationToken => await friendshipRepository.GetCreatedAfterAsync(DateTime.UtcNow.AddDays(-7), cancellationToken) },
            { DateRangeType.Month, async cancellationToken => await friendshipRepository.GetCreatedAfterAsync(DateTime.UtcNow.AddMonths(-1), cancellationToken) },
            { DateRangeType.Year, async cancellationToken => await friendshipRepository.GetCreatedAfterAsync(DateTime.UtcNow.AddYears(-1), cancellationToken) }
        };
    }

}