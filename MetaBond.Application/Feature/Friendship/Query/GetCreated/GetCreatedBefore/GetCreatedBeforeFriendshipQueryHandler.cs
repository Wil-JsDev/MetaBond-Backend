using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Friendship.Query.GetCreated.GetCreatedBefore;

internal sealed class GetCreatedBeforeFriendshipQueryHandler(
    IFriendshipRepository friendshipRepository,
    IDistributedCache decoratedCache,
    ILogger<GetCreatedBeforeFriendshipQueryHandler> logger)
    : IQueryHandler<GetCreatedBeforeFriendshipQuery, PagedResult<FriendshipDTos>>
{
    public async Task<ResultT<PagedResult<FriendshipDTos>>> Handle(
        GetCreatedBeforeFriendshipQuery request,
        CancellationToken cancellationToken)
    {
        var friendshipBefore = GetCreatedBefore(request.PageNumber, request.PageSize);

        if (friendshipBefore.TryGetValue((request.PastDateRangeType), out var beforeAsync))
        {
            string cacheKey =
                $"GetCreatedBeforeFriendshipQueryHandler-{request.PastDateRangeType}-page-{request.PageNumber}-size-{request.PageSize}";
            var result = await decoratedCache.GetOrCreateAsync(
                cacheKey,
                async () =>
                {
                    var friendshipList = await beforeAsync(cancellationToken);

                    if (friendshipList.Items == null) return null;

                    var friendshipDTos = friendshipList.Items.Select(FriendshipMapper.MapFriendshipDTos);

                    PagedResult<FriendshipDTos> pagedResult = new(
                        currentPage: friendshipList.CurrentPage,
                        totalItems: friendshipList.TotalItems,
                        items: friendshipDTos,
                        pageSize: request.PageSize
                    );

                    return pagedResult;
                },
                cancellationToken: cancellationToken);

            if (!result.Items.Any())
            {
                logger.LogError("No friendships found before the specified date range: {PastDateRangeType}",
                    request.PastDateRangeType);

                return ResultT<PagedResult<FriendshipDTos>>.Failure(Error.Failure("400", "The list is empty"));
            }

            logger.LogInformation("Retrieved {Count} friendships before the date range: {PastDateRangeType}",
                result.Items.Count(), request.PastDateRangeType);

            return ResultT<PagedResult<FriendshipDTos>>.Success(result);
        }

        logger.LogError("Failed to retrieve friendships: Invalid date range type {PastDateRangeType}",
            request.PastDateRangeType);

        return ResultT<PagedResult<FriendshipDTos>>.Failure(Error.Failure("400", "Invalid status"));
    }

    #region Private Methods

    private Dictionary<PastDateRangeType, Func<CancellationToken, Task<PagedResult<Domain.Models.Friendship>>>>
        GetCreatedBefore(int pageNumber, int pageSize)
    {
        return new Dictionary<PastDateRangeType, Func<CancellationToken, Task<PagedResult<Domain.Models.Friendship>>>>
        {
            {
                PastDateRangeType.BeforeToday,
                async cancellationToken =>
                    await friendshipRepository.GetCreatedBeforeAsync(DateTime.UtcNow.Date, pageNumber, pageSize,
                        cancellationToken)
            },
            {
                PastDateRangeType.BeforeWeek,
                async cancellationToken =>
                    await friendshipRepository.GetCreatedBeforeAsync(DateTime.UtcNow.AddDays(-7), pageNumber, pageSize,
                        cancellationToken)
            },
            {
                PastDateRangeType.BeforeMonth,
                async cancellationToken =>
                    await friendshipRepository.GetCreatedBeforeAsync(DateTime.UtcNow.AddMonths(-1), pageNumber,
                        pageSize, cancellationToken)
            },
            {
                PastDateRangeType.BeforeYear,
                async cancellationToken =>
                    await friendshipRepository.GetCreatedBeforeAsync(DateTime.UtcNow.AddYears(-1), pageNumber, pageSize,
                        cancellationToken)
            }
        };
    }

    #endregion
}