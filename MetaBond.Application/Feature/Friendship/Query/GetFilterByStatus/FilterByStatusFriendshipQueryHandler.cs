using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Friendship.Query.GetFilterByStatus;

internal sealed class FilterByStatusFriendshipQueryHandler(
    IFriendshipRepository friendshipRepository,
    IDistributedCache decoratedCache,
    ILogger<FilterByStatusFriendshipQueryHandler> logger)
    : IQueryHandler<FilterByStatusFriendshipQuery, PagedResult<FriendshipDTos>>
{
    public async Task<ResultT<PagedResult<FriendshipDTos>>> Handle(
        FilterByStatusFriendshipQuery request,
        CancellationToken cancellationToken)
    {
        var exists = await friendshipRepository.ValidateAsync(x => x.Status == request.Status, cancellationToken);
        if (!exists)
        {
            logger.LogError("No active friendship found with status '{Status}'.", request.Status);

            return ResultT<PagedResult<FriendshipDTos>>.Failure(Error.NotFound("404",
                $"No active friendship exists with status '{request.Status}'."));
        }

        var paginationValidation = PaginationHelper.ValidatePagination<FriendshipDTos>(request.PageNumber,
            request.PageSize, logger);

        if (!paginationValidation.IsSuccess) return paginationValidation.Error!;

        var getStatusFriendship = GetStatusFriendship(request.PageNumber, request.PageSize);
        if (getStatusFriendship.TryGetValue((request.Status), out var statusFilter))
        {
            string cacheKey =
                $"FilterStatusFriendship-{request.Status}-page-{request.PageNumber}-size-{request.PageSize}";
            var friendshipStatusFilter = await decoratedCache.GetOrCreateAsync(
                cacheKey,
                async () =>
                {
                    var status = await statusFilter(cancellationToken);

                    if (status.Items == null) return null;
                    var friendshipDTos = status.Items.Select(FriendshipMapper.MapFriendshipDTos);

                    PagedResult<FriendshipDTos> pagedResult = new(
                        totalItems: status.TotalItems,
                        currentPage: status.CurrentPage,
                        items: friendshipDTos,
                        pageSize: request.PageSize
                    );

                    return pagedResult;
                },
                cancellationToken: cancellationToken);


            if (friendshipStatusFilter is { Items: not null } &&
                !friendshipStatusFilter.Items.Any())
            {
                logger.LogError("No friendships found with status: {Status}", request.Status);

                return ResultT<PagedResult<FriendshipDTos>>.Failure(Error.Failure("400",
                    "No friendships found with the given status"));
            }

            logger.LogInformation("Successfully retrieved {Count} friendships with status: {Status}",
                friendshipStatusFilter.Items.Count(), request.Status);

            return ResultT<PagedResult<FriendshipDTos>>.Success(friendshipStatusFilter);
        }

        logger.LogError("Failed to retrieve friendships: Invalid status {Status}", request.Status);

        return ResultT<PagedResult<FriendshipDTos>>.Failure(Error.Failure("400", "Invalid status"));
    }

    #region Private Methods

    private Dictionary<Status, Func<CancellationToken, Task<PagedResult<Domain.Models.Friendship>>>>
        GetStatusFriendship(int pageNumber, int pageSize)
    {
        return new Dictionary<Status, Func<CancellationToken, Task<PagedResult<Domain.Models.Friendship>>>>()
        {
            {
                (Status.Pending),
                async cancellationToken =>
                    await friendshipRepository.GetFilterByStatusAsync(Status.Pending, pageNumber, pageSize,
                        cancellationToken)
            },
            {
                (Status.Accepted),
                async cancellationToken =>
                    await friendshipRepository.GetFilterByStatusAsync(Status.Accepted, pageNumber, pageSize,
                        cancellationToken)
            },
            {
                (Status.Blocked),
                async cancellationToken =>
                    await friendshipRepository.GetFilterByStatusAsync(Status.Blocked, pageNumber, pageSize,
                        cancellationToken)
            }
        };
    }

    #endregion
}