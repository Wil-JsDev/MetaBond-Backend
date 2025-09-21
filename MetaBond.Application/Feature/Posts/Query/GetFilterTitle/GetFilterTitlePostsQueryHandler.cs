using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Posts;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Posts.Query.GetFilterTitle;

internal sealed class GetFilterTitlePostsQueryHandler(
    IPostsRepository postsRepository,
    ICommunitiesRepository communitiesRepository,
    IDistributedCache decoratedCache,
    ILogger<GetFilterTitlePostsQueryHandler> logger)
    : IQueryHandler<GetFilterTitlePostsQuery, PagedResult<PostsDTos>>
{
    public async Task<ResultT<PagedResult<PostsDTos>>> Handle(
        GetFilterTitlePostsQuery request,
        CancellationToken cancellationToken)
    {
        var community = await EntityHelper.GetEntityByIdAsync(
            communitiesRepository.GetByIdAsync,
            request.CommunitiesId,
            "Communities",
            logger
        );

        if (!community.IsSuccess)
            return community.Error!;

        if (string.IsNullOrEmpty(request.Title))
        {
            logger.LogError("Invalid request: GetFilterTitlePostsQuery request is null.");

            return ResultT<PagedResult<PostsDTos>>.Failure(Error.Failure("400", "Invalid request"));
        }

        var exists = await postsRepository.ValidateAsync(x => x.Title == request.Title, cancellationToken);
        if (!exists)
        {
            logger.LogError("No post found with the title '{Title}'.", request.Title);

            return ResultT<PagedResult<PostsDTos>>.Failure(Error.NotFound("404",
                $"No post exists with the title '{request.Title}'"));
        }

        var paginationValidation =
            PaginationHelper.ValidatePagination<PostsDTos>(request.PageNumber, request.PageSize, logger);

        if (!paginationValidation.IsSuccess) return paginationValidation.Error!;

        string cacheKey =
            $"community-filter-title-{request.CommunitiesId}-{request.Title}-page-{request.PageNumber}-size-{request.PageSize}";
        var result = await decoratedCache.GetOrCreateAsync(
            cacheKey,
            async () =>
            {
                var postsWithTitle = await postsRepository.GetFilterByTitleAsync(
                    request.CommunitiesId,
                    request.Title,
                    request.PageNumber,
                    request.PageSize,
                    cancellationToken);

                var items = postsWithTitle.Items ?? [];

                var postsDTos = items.Select(PostsMapper.PostsToDto);

                PagedResult<PostsDTos> result = new()
                {
                    TotalItems = postsWithTitle.TotalItems,
                    TotalPages = postsWithTitle.TotalPages,
                    CurrentPage = postsWithTitle.CurrentPage,
                    Items = postsDTos
                };

                return result;
            },
            cancellationToken: cancellationToken);

        var items = result.Items ?? [];

        var postsDTosEnumerable = items.ToList();
        if (postsDTosEnumerable.Any())
        {
            logger.LogError("No posts found with the title '{Title}'.", request.Title);

            return ResultT<PagedResult<PostsDTos>>.Failure(Error.Failure("400", "The list is empty"));
        }

        logger.LogInformation("Successfully retrieved {Count} posts with the title '{Title}'.",
            postsDTosEnumerable.Count(), request.Title);

        return ResultT<PagedResult<PostsDTos>>.Success(result);
    }
}