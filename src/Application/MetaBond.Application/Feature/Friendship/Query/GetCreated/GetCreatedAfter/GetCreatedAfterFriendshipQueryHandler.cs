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

namespace MetaBond.Application.Feature.Friendship.Query.GetCreated.GetCreatedAfter;

internal sealed class GetCreatedAfterFriendshipQueryHandler(
    IFriendshipRepository friendshipRepository,
    IDistributedCache decoratedCache,
    ILogger<GetCreatedAfterFriendshipQueryHandler> logger)
    : IQueryHandler<GetCreatedAfterFriendshipQuery, PagedResult<FriendshipDTos>>
{
    public async Task<ResultT<PagedResult<FriendshipDTos>>> Handle(
        GetCreatedAfterFriendshipQuery request,
        CancellationToken cancellationToken)
    {
        var validationPagination =
            PaginationHelper.ValidatePagination<FriendshipDTos>(request.PageNumber, request.PageSize, logger);

        if (!validationPagination.IsSuccess)
            return ResultT<PagedResult<FriendshipDTos>>.Failure(validationPagination.Error!);

        var friendshipAfter = GetCreatedAfterFriendship(request.PageNumber, request.PageSize);
        if (friendshipAfter.TryGetValue((request.DateRange), out var createdAfter))
        {
            string cacheKey =
                $"GetCreatedAfterFriendshipQuery-{request.DateRange}-page-{request.PageNumber}-size-{request.PageSize}";
            var result = await decoratedCache.GetOrCreateAsync(
                cacheKey,
                async () =>
                {
                    var friendshipList = await createdAfter(cancellationToken);

                    if (friendshipList.Items == null) return null;
                    var friendshipDTos = friendshipList.Items.Select(FriendshipMapper.MapFriendshipDTos);

                    var pagedResult = new PagedResult<FriendshipDTos>(
                        totalItems: friendshipList.TotalItems,
                        currentPage: friendshipList.CurrentPage,
                        items: friendshipDTos,
                        pageSize: request.PageSize
                    );

                    return pagedResult;
                },
                cancellationToken: cancellationToken);

            if (!result.Items.Any())
            {
                logger.LogError("No friendships found after the specified date range: {DateRange}", request.DateRange);

                return ResultT<PagedResult<FriendshipDTos>>.Failure(Error.Failure("400", "The list is empty"));
            }

            logger.LogInformation("Retrieved {Count} friendships after the date range: {DateRange}",
                result.Items.Count(), request.DateRange);

            return ResultT<PagedResult<FriendshipDTos>>.Success(result);
        }

        logger.LogError("Failed to retrieve friendships: Invalid date range type {DateRange}", request.DateRange);

        return ResultT<PagedResult<FriendshipDTos>>.Failure(Error.Failure("400", "Invalid status"));
    }

    #region Private Methods

    private Dictionary<DateRangeType, Func<CancellationToken, Task<PagedResult<Domain.Models.Friendship>>>>
        GetCreatedAfterFriendship(int pageNumber, int pageSize)
    {
        return new Dictionary<DateRangeType, Func<CancellationToken, Task<PagedResult<Domain.Models.Friendship>>>>()
        {
            {
                DateRangeType.Today,
                async cancellationToken =>
                    await friendshipRepository.GetCreatedAfterAsync(DateTime.UtcNow.Date, pageNumber, pageSize,
                        cancellationToken)
            },
            {
                DateRangeType.Week,
                async cancellationToken =>
                    await friendshipRepository.GetCreatedAfterAsync(DateTime.UtcNow.AddDays(-7), pageNumber, pageSize,
                        cancellationToken)
            },
            {
                DateRangeType.Month,
                async cancellationToken =>
                    await friendshipRepository.GetCreatedAfterAsync(DateTime.UtcNow.AddMonths(-1), pageNumber, pageSize,
                        cancellationToken)
            },
            {
                DateRangeType.Year,
                async cancellationToken =>
                    await friendshipRepository.GetCreatedAfterAsync(DateTime.UtcNow.AddYears(-1), pageNumber, pageSize,
                        cancellationToken)
            }
        };
    }

    #endregion
}