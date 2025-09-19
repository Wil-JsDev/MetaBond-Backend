using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Friendship.Query.GetFriendshipWithUser;

internal sealed class GetFriendshipWithUsersQueryHandler(
    IFriendshipRepository friendshipRepository,
    ILogger<GetFriendshipWithUsersQueryHandler> logger,
    IDistributedCache decoratedCache)
    : IQueryHandler<GetFriendshipWithUsersQuery, PagedResult<FriendshipWithUserDTos>>
{
    public async Task<ResultT<PagedResult<FriendshipWithUserDTos>>> Handle(
        GetFriendshipWithUsersQuery request,
        CancellationToken cancellationToken)
    {
        var friendship =
            await EntityHelper.GetEntityByIdAsync(friendshipRepository.GetByIdAsync,
                request.FriendshipId ?? Guid.Empty,
                "Friendship",
                logger);

        if (!friendship.IsSuccess) return friendship.Error!;

        var getFriendshipWithUser =
            await decoratedCache.GetOrCreateAsync(
                $"GetFriendshipWithUsersQueryCache-{request.FriendshipId}-page-{request.PageNumber}-size-{request.PageSize}",
                async () =>
                {
                    var friendshipUser = await friendshipRepository.GetFriendshipWithUsersAsync(
                        (Guid)request.FriendshipId!,
                        request.PageNumber,
                        request.PageSize,
                        cancellationToken);

                    var pagedDto = friendshipUser.Items.ToFriendshipWithUserDtos().ToList();

                    PagedResult<FriendshipWithUserDTos> pagedResult = new(
                        totalItems: friendshipUser.TotalItems,
                        currentPage: friendshipUser.CurrentPage,
                        items: pagedDto,
                        pageSize: request.PageSize
                    );

                    return pagedResult;
                },
                cancellationToken: cancellationToken);

        if (!getFriendshipWithUser.Items.Any())
        {
            logger.LogWarning("No friendship with users found for ID: {FriendshipId}", request.FriendshipId);

            return ResultT<PagedResult<FriendshipWithUserDTos>>.Failure(Error.Failure("400",
                "No friendship with users found for the given ID."));
        }

        logger.LogInformation("Friendship with users loaded successfully for ID: {FriendshipId}", request.FriendshipId);

        return ResultT<PagedResult<FriendshipWithUserDTos>>.Success(getFriendshipWithUser);
    }
}