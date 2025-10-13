using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Posts;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Posts.Query.GetFilterRecent;

internal sealed class GetFilterRecentPostsQueryHandler(
    IPostsRepository postsRepository,
    IDistributedCache decoratedCache,
    ILogger<GetFilterRecentPostsQueryHandler> logger)
    : IQueryHandler<GetFilterRecentPostsQuery, PagedResult<PostsDTos>>
{
    public async Task<ResultT<PagedResult<PostsDTos>>> Handle(
        GetFilterRecentPostsQuery request,
        CancellationToken cancellationToken)
    {
        var paginationValidation =
            PaginationHelper.ValidatePagination<PostsDTos>(request.PageNumber, request.PageSize, logger);
        if (!paginationValidation.IsSuccess) return paginationValidation.Error!;

        string cacheKey = $"posts:recent:{request.CommunitiesId}:p{request.PageNumber}:s{request.PageSize}";

        var result = await decoratedCache.GetOrCreateAsync(
            cacheKey,
            async () =>
            {
                var postsFilters = await postsRepository.FilterRecentPostsByCountAsync(
                    request.CommunitiesId,
                    request.PageNumber,
                    request.PageSize,
                    cancellationToken);

                var items = postsFilters.Items ?? [];

                var postsDtos = items.Select(PostsMapper.PostsToDto).ToList();

                return new PagedResult<PostsDTos>(
                    totalItems: postsFilters.TotalItems,
                    currentPage: postsFilters.CurrentPage,
                    pageSize: request.PageSize,
                    items: postsDtos
                );
            },
            cancellationToken: cancellationToken);

        if (!result.Items.Any())
        {
            logger.LogWarning(
                "No posts found for Community {CommunityId}, Page {Page}, Size {Size}",
                request.CommunitiesId, request.PageNumber, request.PageSize);

            return ResultT<PagedResult<PostsDTos>>.Failure(Error.Failure("404", "No posts found"));
        }

        logger.LogInformation(
            "Retrieved {Count} recent posts for Community {CommunityId}, Page {Page}, Size {Size}",
            result.Items.Count(), request.CommunitiesId, request.PageNumber, request.PageSize);

        return ResultT<PagedResult<PostsDTos>>.Success(result);
    }
}