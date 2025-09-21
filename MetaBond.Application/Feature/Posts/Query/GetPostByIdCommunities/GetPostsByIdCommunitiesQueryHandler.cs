using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.DTOs.Posts;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Posts.Query.GetPostByIdCommunities;

internal sealed class GetPostsByIdCommunitiesQueryHandler(
    IPostsRepository postsRepository,
    IDistributedCache decoratedCache,
    ILogger<GetPostsByIdCommunitiesQueryHandler> logger)
    : IQueryHandler<GetPostsByIdCommunitiesQuery, PagedResult<PostsWithCommunitiesDTos>>
{
    public async Task<ResultT<PagedResult<PostsWithCommunitiesDTos>>> Handle(
        GetPostsByIdCommunitiesQuery request,
        CancellationToken cancellationToken)
    {
        var posts = await EntityHelper.GetEntityByIdAsync(
            postsRepository.GetByIdAsync,
            request.PostsId,
            "Posts",
            logger);
        if (!posts.IsSuccess) return posts.Error!;

        var paginationValidation = PaginationHelper.ValidatePagination<PostsWithCommunitiesDTos>(request.PageNumber,
            request.PageSize, logger);

        if (!paginationValidation.IsSuccess) return paginationValidation.Error!;

        string cacheKey =
            $"get-posts-by-id-details-{request.PostsId}-page-{request.PageNumber}-size-{request.PageSize}";
        var postsWithCommunities = await decoratedCache.GetOrCreateAsync(
            cacheKey,
            async () =>
            {
                var postsWithCommunity = await postsRepository.GetPostsByIdWithCommunitiesAsync(
                    request.PostsId,
                    request.PageNumber,
                    request.PageSize,
                    cancellationToken);

                var items = postsWithCommunity.Items ?? [];

                var postsWithCommunitiesDTos = items.ToPostsWithCommunitiesDtos();

                PagedResult<PostsWithCommunitiesDTos> result = new()
                {
                    TotalItems = postsWithCommunity.TotalItems,
                    TotalPages = postsWithCommunity.TotalPages,
                    CurrentPage = postsWithCommunity.CurrentPage,
                    Items = postsWithCommunitiesDTos
                };

                return result;
            },
            cancellationToken: cancellationToken);

        var itemsList = postsWithCommunities.Items ?? [];

        if (!itemsList.Any())
        {
            logger.LogError("No communities found for post with ID '{PostsId}'.", request.PostsId);

            return ResultT<PagedResult<PostsWithCommunitiesDTos>>.Failure(Error.Failure("400", "The list is empty"));
        }

        logger.LogInformation(
            "Successfully retrieved {Count} posts with their associated communities for post ID '{PostsId}'.",
            itemsList.Count(), request.PostsId);

        return ResultT<PagedResult<PostsWithCommunitiesDTos>>.Success(postsWithCommunities);
    }
}