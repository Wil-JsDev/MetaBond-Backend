using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Friendship.Query.GetOrderById;

internal sealed class GetOrderByIdFriendshipQueryHandler(
    IFriendshipRepository friendshipRepository,
    IDistributedCache decoratedCache,
    ILogger<GetOrderByIdFriendshipQueryHandler> logger)
    : IQueryHandler<GetOrderByIdFriendshipQuery, PagedResult<FriendshipDTos>>
{
    public async Task<ResultT<PagedResult<FriendshipDTos>>> Handle(
        GetOrderByIdFriendshipQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Sort))
        {
            logger.LogError("Sort parameter is required.");

            return ResultT<PagedResult<FriendshipDTos>>.Failure(Error.Failure("400", "Sort parameter is required."));
        }

        var paginationValidation = PaginationHelper.ValidatePagination<FriendshipDTos>(request.PageNumber,
            request.PageSize, logger);

        if (!paginationValidation.IsSuccess) return paginationValidation.Error!;

        var friendshipSort = GetSort(request.PageNumber, request.PageSize);
        if (friendshipSort.TryGetValue((request.Sort!.ToUpper()), out var getSortFriendship))
        {
            string cacheKey = $"GetOrderByIdFriendshipQuery-{request.Sort}";
            var friendshipList = await decoratedCache.GetOrCreateAsync(
                cacheKey,
                async () =>
                {
                    var sortFriendship = await getSortFriendship(cancellationToken);

                    if (sortFriendship.Items != null)
                    {
                        var friendshipDTos = sortFriendship.Items.Select(FriendshipMapper.MapFriendshipDTos);

                        PagedResult<FriendshipDTos> pagedResult = new(
                            totalItems: sortFriendship.TotalItems,
                            currentPage: sortFriendship.CurrentPage,
                            items: friendshipDTos,
                            pageSize: request.PageSize
                        );

                        return pagedResult;
                    }

                    return null;
                },
                cancellationToken: cancellationToken);

            if (!friendshipList.Items.Any())
            {
                logger.LogError("No friendships found for the sorted order: {Sort}", request.Sort);

                return ResultT<PagedResult<FriendshipDTos>>.Failure(Error.Failure("400", "The list is empty"));
            }

            logger.LogInformation("Successfully retrieved {Count} friendships sorted by: {Sort}",
                friendshipList.Items.Count(), request.Sort);

            return ResultT<PagedResult<FriendshipDTos>>.Success(friendshipList);
        }

        logger.LogError("Invalid order parameter: {Sort}. Expected 'asc' or 'desc'.", request.Sort);

        return ResultT<PagedResult<FriendshipDTos>>.Failure
            (Error.Failure("400", "Invalid order parameter. Please specify 'asc' or 'desc'."));
    }

    #region Private Methods

    private Dictionary<string, Func<CancellationToken, Task<PagedResult<Domain.Models.Friendship>>>> GetSort(
        int pageNumber, int pageSize)
    {
        return new Dictionary<string, Func<CancellationToken, Task<PagedResult<Domain.Models.Friendship>>>>
        {
            {
                "asc",
                async cancellationToken =>
                    await friendshipRepository.OrderByIdAscAsync(pageNumber, pageSize, cancellationToken)
            },
            {
                "desc",
                async cancellationToken =>
                    await friendshipRepository.OrderByIdDescAsync(pageNumber, pageSize, cancellationToken)
            }
        };
    }

    #endregion
}