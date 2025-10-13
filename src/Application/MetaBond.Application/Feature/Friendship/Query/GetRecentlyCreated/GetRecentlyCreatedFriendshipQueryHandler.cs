using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Friendship.Query.GetRecentlyCreated;

internal sealed class GetRecentlyCreatedFriendshipQueryHandler(
    IFriendshipRepository friendshipRepository,
    IDistributedCache decoratedCache,
    ILogger<GetRecentlyCreatedFriendshipQueryHandler> logger)
    : IQueryHandler<GetRecentlyCreatedFriendshipQuery, PagedResult<FriendshipDTos>>
{
    public async Task<ResultT<PagedResult<FriendshipDTos>>> Handle(
        GetRecentlyCreatedFriendshipQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Limit <= 0)
        {
            logger.LogError("Invalid limit: {Limit}", request.Limit);

            return ResultT<PagedResult<FriendshipDTos>>.Failure(
                Error.Failure("400", "Invalid limit. Limit must be greater than zero."));
        }

        var validationPagination =
            PaginationHelper.ValidatePagination<FriendshipDTos>(request.PageNumber, request.PageSize, logger);

        if (!validationPagination.IsSuccess) return validationPagination.Error!;

        string cacheKey =
            $"GetRecentlyCreatedFriendship-{request.Limit}-page-{request.PageNumber}-size-{request.PageSize}";
        var friendshipRecently = await decoratedCache.GetOrCreateAsync(
            cacheKey,
            async () =>
            {
                var recentFriendship =
                    await friendshipRepository.GetRecentlyCreatedAsync(request.Limit, request.PageNumber,
                        request.PageSize, cancellationToken);

                if (recentFriendship.Items == null) return null;
                var friendshipDTos = recentFriendship.Items.Select(FriendshipMapper.MapFriendshipDTos).ToList();

                PagedResult<FriendshipDTos> value = new(
                    totalItems: recentFriendship.TotalItems,
                    currentPage: recentFriendship.CurrentPage,
                    items: friendshipDTos,
                    pageSize: request.PageSize
                );

                return value;
            },
            cancellationToken: cancellationToken);

        if (!friendshipRecently.Items.Any())
        {
            logger.LogError("No recent friendships found for the given limit: {Limit}", request.Limit);

            return ResultT<PagedResult<FriendshipDTos>>.Failure(
                Error.Failure("400", "No recent friendships found."));
        }

        logger.LogInformation("Successfully retrieved {Count} recent friendships with limit: {Limit}",
            friendshipRecently.Items.Count(), request.Limit);

        return ResultT<PagedResult<FriendshipDTos>>.Success(friendshipRecently);
    }
}