using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Interfaces.Repository;
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
        if (request != null)
        {
            string cacheKey = $"paged-friendship-page-{request.PageNumber}-size-{request.PageSize}";;
            var pagedFriendship = await decoratedCache.GetOrCreateAsync(
                cacheKey,
                async () => await friendshipRepository.GetPagedFriendshipAsync(request.PageNumber, request.PageSize,
                    cancellationToken), 
                cancellationToken: cancellationToken);

            var friendshipDto = pagedFriendship.Items!.Select(x => new FriendshipDTos
            (
                FriendshipId: x.Id,
                Status: x.Status,
                RequesterId: x.RequesterId,
                AddresseeId: x.AddresseeId,
                CreatedAt: x.CreateAdt
            ));

            IEnumerable<FriendshipDTos> friendshipDTosEnumerable = friendshipDto.ToList();
            if (!friendshipDTosEnumerable.Any())
            {
                logger.LogError("No friendships found for page {PageNumber} with page size {PageSize}.", 
                    request.PageNumber, request.PageSize);

                return ResultT<PagedResult<FriendshipDTos>>.Failure(Error.Failure("400", "No friendships were found for the specified criteria."));
            }

            PagedResult<FriendshipDTos> result = new()
            {
                TotalItems = pagedFriendship.TotalItems,
                CurrentPage = pagedFriendship.CurrentPage,
                TotalPages = pagedFriendship.TotalPages,
                Items = friendshipDTosEnumerable
            };

            logger.LogInformation("Successfully retrieved page {PageNumber} of friendships with page size {PageSize}. Total items: {TotalItems}, Total pages: {TotalPages}",
                request.PageNumber, request.PageSize, pagedFriendship.TotalItems, pagedFriendship.TotalPages);

            return ResultT<PagedResult<FriendshipDTos>>.Success(result);
        }

        logger.LogError("Invalid request: Request parameters are null or incorrect");

        return ResultT<PagedResult<FriendshipDTos>>.Failure
            (Error.Failure("400", "No friendship were found for the specified criteria."));
    }
}