using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Posts;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Posts.Query.Pagination;

internal class GetPagedPostsQueryHandler(
    IPostsRepository postsRepository,
    IDistributedCache decoratedCache,
    ILogger<GetPagedPostsQueryHandler> logger)
    : IQueryHandler<GetPagedPostsQuery, PagedResult<PostsDTos>>
{
    public async Task<ResultT<PagedResult<PostsDTos>>> Handle(
        GetPagedPostsQuery request,
        CancellationToken cancellationToken)
    {
        var validationPaginationResult = PaginationHelper.ValidatePagination<PostsDTos>(
            request.PageNumber,
            request.PageSize,
            logger
        );
        if (!validationPaginationResult.IsSuccess)
            return validationPaginationResult.Error!;

        string cacheKey = $"get-paged-posts-{request.PageNumber}-size-{request.PageSize}";

        var pagedPosts = await decoratedCache.GetOrCreateAsync(
            cacheKey,
            async () =>
            {
                var pagedResultPosts = await postsRepository.GetPagedPostsAsync(
                    request.PageNumber,
                    request.PageSize,
                    cancellationToken);

                var postsDto = pagedResultPosts.Items!.Select(PostsMapper.PostsToDto);

                PagedResult<PostsDTos> result = new()
                {
                    TotalItems = pagedResultPosts.TotalItems,
                    TotalPages = pagedResultPosts.TotalPages,
                    CurrentPage = pagedResultPosts.CurrentPage,
                    Items = postsDto
                };

                return result;
            },
            cancellationToken: cancellationToken);

        if (!pagedPosts.Items!.Any())
        {
            logger.LogError("No posts found for the requested page: {PageNumber}, PageSize: {PageSize}",
                request.PageNumber, request.PageSize);

            return ResultT<PagedResult<PostsDTos>>.Failure(Error.Failure("400", ""));
        }

        logger.LogInformation("Retrieved {TotalItems} posts for page {CurrentPage} of {TotalPages}.",
            pagedPosts.TotalItems, pagedPosts.CurrentPage, pagedPosts.TotalPages);

        return ResultT<PagedResult<PostsDTos>>.Success(pagedPosts);
    }
}