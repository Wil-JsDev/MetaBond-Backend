using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.DTOs.Posts;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Posts.Query.GetPostWithAuthor;

internal sealed class GetPostWithAuthorQueryHandler(
    IPostsRepository postsRepository,
    ILogger<GetPostWithAuthorQueryHandler> logger,
    IDistributedCache decoratedCache
) : IQueryHandler<GetPostWithAuthorQuery, PagedResult<PostsWithUserDTos>>
{
    public async Task<ResultT<PagedResult<PostsWithUserDTos>>> Handle(
        GetPostWithAuthorQuery request,
        CancellationToken cancellationToken)
    {
        var posts = await EntityHelper.GetEntityByIdAsync(
            postsRepository.GetByIdAsync,
            request.PostsId ?? Guid.Empty,
            "Posts",
            logger);
        if (!posts.IsSuccess)
            return ResultT<PagedResult<PostsWithUserDTos>>.Failure(posts.Error!);

        var validationPagination = PaginationHelper.ValidatePagination<PostsWithUserDTos>(request.PageNumber,
            request.PageSize, logger);

        if (!validationPagination.IsSuccess)
            return validationPagination.Error!;

        var postsWithAuthor = await decoratedCache.GetOrCreateAsync(
            $"GetPostsWithAuthor-{request.PostsId}-page-{request.PageNumber}-size-{request.PageSize}",
            async () =>
            {
                var postsWithAuthors = await postsRepository.GetPostWithAuthorAsync(request.PostsId ?? Guid.Empty,
                    request.PageNumber,
                    request.PageSize,
                    cancellationToken);

                var items = postsWithAuthors.Items ?? [];

                var postsWithUser = items.ToPostsWithUserDtos();

                PagedResult<PostsWithUserDTos> result = new()
                {
                    TotalItems = postsWithAuthors.TotalItems,
                    TotalPages = postsWithAuthors.TotalPages,
                    CurrentPage = postsWithAuthors.CurrentPage,
                    Items = postsWithUser
                };

                return result;
            },
            cancellationToken: cancellationToken);

        var postsWithUserDTosEnumerable = postsWithAuthor.Items ?? [];

        if (!postsWithUserDTosEnumerable.Any())
        {
            logger.LogWarning("No posts found with the specified PostId: {PostId}", request.PostsId);

            return ResultT<PagedResult<PostsWithUserDTos>>.Failure(Error.Failure("400",
                "No posts found with the given PostId."));
        }

        logger.LogInformation("Successfully retrieved and mapped {Count} posts with author info.",
            postsWithUserDTosEnumerable.Count());

        return ResultT<PagedResult<PostsWithUserDTos>>.Success(postsWithAuthor);
    }
}