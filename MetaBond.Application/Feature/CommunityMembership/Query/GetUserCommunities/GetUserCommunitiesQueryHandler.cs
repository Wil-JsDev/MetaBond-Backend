using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communities;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.CommunityMembership.Query.GetUserCommunities;

internal sealed class GetUserCommunitiesQueryHandler(
    ILogger<GetUserCommunitiesQueryHandler> logger,
    ICommunityMembershipRepository communityMembershipRepository,
    IUserRepository userRepository,
    IDistributedCache cache
) : IQueryHandler<GetUserCommunitiesQuery, PagedResult<CommunitiesDTos>>
{
    public async Task<ResultT<PagedResult<CommunitiesDTos>>> Handle(GetUserCommunitiesQuery request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogError("GetUserCommunitiesQuery request is null.");

            return ResultT<PagedResult<CommunitiesDTos>>.Failure(Error.Failure("400",
                "Invalid request: GetUserCommunitiesQuery cannot be null."));
        }

        var validationPagination = PaginationHelper.ValidatePagination<CommunitiesDTos>(request.PageNumber,
            request.PageSize, logger);

        if (!validationPagination.IsSuccess)
            return validationPagination;

        var userResult = await EntityHelper.GetEntityByIdAsync(userRepository.GetByIdAsync,
            request.UserId ?? Guid.Empty,
            "User",
            logger
        );

        if (!userResult.IsSuccess) return ResultT<PagedResult<CommunitiesDTos>>.Failure(userResult.Error!);

        var result = await cache.GetOrCreateAsync($"get-user-communities-{request.UserId}", async () =>
        {
            var userCommunityPaged =
                await communityMembershipRepository.GetUserCommunitiesAsync(request.UserId ?? Guid.Empty,
                    request.PageNumber, request.PageSize, cancellationToken);

            var dtoItems = userCommunityPaged.Items!
                .Select(CommunityMembershipMapper.CommunityMembershipToCommunitiesDTos).ToList();

            PagedResult<CommunitiesDTos> pagedResult = new()
            {
                TotalItems = userCommunityPaged.TotalItems,
                CurrentPage = userCommunityPaged.CurrentPage,
                TotalPages = userCommunityPaged.TotalPages,
                Items = dtoItems
            };

            return pagedResult;
        }, cancellationToken: cancellationToken);

        if (!result.Items!.Any())
        {
            logger.LogInformation("User {UserId} has no communities.", request.UserId);

            return ResultT<PagedResult<CommunitiesDTos>>.Failure(Error.Failure("400", "User has no communities."));
        }

        logger.LogInformation("Successfully retrieved {TotalItems} communities (Page {CurrentPage} of {TotalPages}).",
            result.TotalItems, result.CurrentPage, result.TotalPages);

        return ResultT<PagedResult<CommunitiesDTos>>.Success(result);
    }
}