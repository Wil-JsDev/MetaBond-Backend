using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Posts;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Posts.Query.GetPagedPostsByUserCommunity;

internal sealed class
    GetPagedPostsByUserCommunityQueryHandler(
        ILogger<GetPagedPostsByUserCommunityQueryHandler> logger,
        IPostsRepository postsRepository,
        ICommunitiesRepository communitiesRepository,
        IUserRepository userRepository,
        IDistributedCache decoratedCache
    ) : IQueryHandler<GetPagedPostsByUserCommunityQuery, PagedResult<PostsDTos>>
{
    public async Task<ResultT<PagedResult<PostsDTos>>> Handle(GetPagedPostsByUserCommunityQuery request,
        CancellationToken cancellationToken)
    {
        var paginationValidation = PaginationHelper.ValidatePagination<PostsDTos>(request.PageNumber,
            request.PageSize, logger);

        if (!paginationValidation.IsSuccess)
            return ResultT<PagedResult<PostsDTos>>.Failure(paginationValidation.Error!);

        var userResult = await EntityHelper.GetEntityByIdAsync(
            userRepository.GetByIdAsync,
            request.CreatedById ?? Guid.Empty,
            "User",
            logger
        );

        if (!userResult.IsSuccess) return ResultT<PagedResult<PostsDTos>>.Failure(userResult.Error!);

        var communityResult = await EntityHelper.GetEntityByIdAsync(
            communitiesRepository.GetByIdAsync,
            request.CommunitiesId ?? Guid.Empty,
            "Community",
            logger
        );

        if (!communityResult.IsSuccess) return ResultT<PagedResult<PostsDTos>>.Failure(communityResult.Error!);

        string key =
            $"user-community-posts-{request.CreatedById}-{request.CommunitiesId}-{request.PageNumber}-{request.PageSize}";
        var result = await decoratedCache.GetOrCreateAsync(key, async () =>
        {
            var pagedPosts = await postsRepository.GetPagedPostsByUserAndCommunitiesAsync(
                request.CreatedById ?? Guid.Empty,
                request.CommunitiesId ?? Guid.Empty,
                request.PageNumber,
                request.PageSize,
                cancellationToken
            );

            var items = pagedPosts.Items ?? [];

            var pagedPostsDto = items.Select(PostsMapper.PostsToDto).ToList();

            var pagedResponse = new PagedResult<PostsDTos>()
            {
                CurrentPage = pagedPosts.CurrentPage,
                TotalPages = pagedPosts.TotalPages,
                TotalItems = pagedPosts.TotalItems,
                Items = pagedPostsDto
            };

            return pagedResponse;
        }, cancellationToken: cancellationToken);

        if (result.Items != null && result.Items.Any()) return ResultT<PagedResult<PostsDTos>>.Success(result);

        logger.LogInformation(
            "No posts found for User ID {UserId} in Community ID {CommunityId} (Page: {Page})",
            request.CreatedById,
            request.CommunitiesId,
            request.PageNumber);

        return ResultT<PagedResult<PostsDTos>>.Failure(Error.Failure("404", "No posts for user ID in community ID"));
    }
}