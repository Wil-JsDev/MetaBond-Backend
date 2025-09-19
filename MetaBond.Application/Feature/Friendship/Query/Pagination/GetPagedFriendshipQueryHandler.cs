using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Friendship.Query.Pagination;

internal sealed class GetPagedFriendshipQueryHandler(
    IFriendshipRepository friendshipRepository,
    IDistributedCache decoratedCache,
    ILogger<GetPagedFriendshipQueryHandler> logger)
    : IQueryHandler<GetPagedFriendshipQuery, PagedResult<FriendshipDTos>>
{
    public async Task<ResultT<PagedResult<FriendshipDTos>>> Handle(
        GetPagedFriendshipQuery? request,
        CancellationToken cancellationToken)
    {
        var validationPagination = PaginationHelper.ValidatePagination<FriendshipDTos>
        (
            request!.PageNumber,
            request.PageSize,
            logger
        );

        if (!validationPagination.IsSuccess)
            return validationPagination;

        string cacheKey = $"paged-friendship-page-{request!.PageNumber}-size-{request.PageSize}";

        var pagedFriendship = await decoratedCache.GetOrCreateAsync(
            cacheKey,
            async () =>
            {
                var resultPaged = await friendshipRepository.GetPagedFriendshipAsync(request.PageNumber,
                    request.PageSize,
                    cancellationToken);

                if (resultPaged.Items == null) return null;
                var friendshipDto = resultPaged.Items.Select(FriendshipMapper.MapFriendshipDTos);

                PagedResult<FriendshipDTos> result = new()
                {
                    TotalItems = resultPaged.TotalItems,
                    CurrentPage = resultPaged.CurrentPage,
                    TotalPages = resultPaged.TotalPages,
                    Items = friendshipDto
                };

                return result;
            },
            cancellationToken: cancellationToken);

        if (pagedFriendship is { Items: not null } && !pagedFriendship.Items.Any())
        {
            logger.LogError("No friendships found for page {PageNumber} with page size {PageSize}.",
                request.PageNumber, request.PageSize);

            return ResultT<PagedResult<FriendshipDTos>>.Failure(Error.Failure("400",
                "No friendships were found for the specified criteria."));
        }

        logger.LogInformation(
            "Successfully retrieved page {PageNumber} of friendships with page size {PageSize}. Total items: {TotalItems}, Total pages: {TotalPages}",
            request.PageNumber, request.PageSize, pagedFriendship.TotalItems, pagedFriendship.TotalPages);

        return ResultT<PagedResult<FriendshipDTos>>.Success(pagedFriendship);
    }
}