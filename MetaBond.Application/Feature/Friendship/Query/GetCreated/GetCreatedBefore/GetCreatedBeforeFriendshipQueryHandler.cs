using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Friendship.Query.GetCreated.GetCreatedBefore;

internal sealed class GetCreatedBeforeFriendshipQueryHandler(
    IFriendshipRepository friendshipRepository,
    IDistributedCache decoratedCache,
    ILogger<GetCreatedBeforeFriendshipQueryHandler> logger)
    : IQueryHandler<GetCreatedBeforeFriendshipQuery, IEnumerable<FriendshipDTos>>
{
    public async Task<ResultT<IEnumerable<FriendshipDTos>>> Handle(
        GetCreatedBeforeFriendshipQuery request,
        CancellationToken cancellationToken)
    {
        var friendshipBefore = GetCreatedBefore();

        if (friendshipBefore.TryGetValue((request.PastDateRangeType), out var beforeAsync))
        {
            string cacheKey = $"GetCreatedBeforeFriendshipQueryHandler-{request.PastDateRangeType}";
            var result = await decoratedCache.GetOrCreateAsync(
                cacheKey,
                async () =>
                {
                    var friendshipList = await beforeAsync(cancellationToken);

                    var friendshipDTos = friendshipList.Select(FriendshipMapper.MapFriendshipDTos);

                    return friendshipDTos;
                }
                ,
                cancellationToken: cancellationToken);

            if (!result.Any())
            {
                logger.LogError("No friendships found before the specified date range: {PastDateRangeType}",
                    request.PastDateRangeType);

                return ResultT<IEnumerable<FriendshipDTos>>.Failure(Error.Failure("400", "The list is empty"));
            }

            logger.LogInformation("Retrieved {Count} friendships before the date range: {PastDateRangeType}",
                result.Count(), request.PastDateRangeType);

            return ResultT<IEnumerable<FriendshipDTos>>.Success(result);
        }

        logger.LogError("Failed to retrieve friendships: Invalid date range type {PastDateRangeType}",
            request.PastDateRangeType);

        return ResultT<IEnumerable<FriendshipDTos>>.Failure(Error.Failure("400", "Invalid status"));
    }

    #region Private Methods

    private Dictionary<PastDateRangeType, Func<CancellationToken, Task<IEnumerable<Domain.Models.Friendship>>>>
        GetCreatedBefore()
    {
        return new Dictionary<PastDateRangeType, Func<CancellationToken, Task<IEnumerable<Domain.Models.Friendship>>>>
        {
            {
                PastDateRangeType.BeforeToday,
                async cancellationToken =>
                    await friendshipRepository.GetCreatedBeforeAsync(DateTime.UtcNow.Date, cancellationToken)
            },
            {
                PastDateRangeType.BeforeWeek,
                async cancellationToken =>
                    await friendshipRepository.GetCreatedBeforeAsync(DateTime.UtcNow.AddDays(-7), cancellationToken)
            },
            {
                PastDateRangeType.BeforeMonth,
                async cancellationToken =>
                    await friendshipRepository.GetCreatedBeforeAsync(DateTime.UtcNow.AddMonths(-1), cancellationToken)
            },
            {
                PastDateRangeType.BeforeYear,
                async cancellationToken =>
                    await friendshipRepository.GetCreatedBeforeAsync(DateTime.UtcNow.AddYears(-1), cancellationToken)
            }
        };
    }

    #endregion
}