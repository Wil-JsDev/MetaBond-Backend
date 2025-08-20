using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.DTOs.Posts;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Posts.Query.GetPostWithAuthor;

internal sealed class GetPostWithAuthorQueryHandler(
    IPostsRepository postsRepository,
    ILogger<GetPostWithAuthorQueryHandler> logger,
    IDistributedCache decoratedCache
) : IQueryHandler<GetPostWithAuthorQuery, IEnumerable<PostsWithUserDTos>>
{
    public async Task<ResultT<IEnumerable<PostsWithUserDTos>>> Handle(
        GetPostWithAuthorQuery request,
        CancellationToken cancellationToken)
    {
        var posts = await postsRepository.GetByIdAsync(request.PostsId ?? Guid.Empty);
        if (posts is null)
        {
            logger.LogWarning("Post with ID '{PostsId}' not found.", request.PostsId);

            return ResultT<IEnumerable<PostsWithUserDTos>>.Failure(Error.NotFound("404",
                $"{request.PostsId} not found"));
        }

        var postsWithAuthor = await decoratedCache.GetOrCreateAsync($"GetPostsWithAuthor-{request.PostsId}",
            async () =>
            {
                var postsWithAuthors = await postsRepository.GetPostWithAuthorAsync(request.PostsId ?? Guid.Empty,
                    cancellationToken);

                var postsWithUser = postsWithAuthors.ToPostsWithUserDtos();

                return postsWithUser;
            },
            cancellationToken: cancellationToken);

        IEnumerable<PostsWithUserDTos> postsWithUserDTosEnumerable = postsWithAuthor.ToList();
        if (!postsWithUserDTosEnumerable.Any())
        {
            logger.LogWarning("No posts found with the specified PostId: {PostId}", request.PostsId);

            return ResultT<IEnumerable<PostsWithUserDTos>>.Failure(Error.Failure("400",
                "No posts found with the given PostId."));
        }

        logger.LogInformation("Successfully retrieved and mapped {Count} posts with author info.",
            postsWithUserDTosEnumerable.Count());

        return ResultT<IEnumerable<PostsWithUserDTos>>.Success(postsWithUserDTosEnumerable);
    }
}