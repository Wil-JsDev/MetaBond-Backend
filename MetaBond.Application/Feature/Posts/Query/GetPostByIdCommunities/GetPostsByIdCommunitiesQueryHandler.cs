using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.DTOs.Posts;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Posts.Query.GetPostByIdCommunities;

internal sealed class GetPostsByIdCommunitiesQueryHandler(
    IPostsRepository postsRepository,
    IDistributedCache decoratedCache,
    ILogger<GetPostsByIdCommunitiesQueryHandler> logger)
    : IQueryHandler<GetPostsByIdCommunitiesQuery, IEnumerable<PostsWithCommunitiesDTos>>
{
    public async Task<ResultT<IEnumerable<PostsWithCommunitiesDTos>>> Handle(
        GetPostsByIdCommunitiesQuery request,
        CancellationToken cancellationToken)
    {
        var posts = await EntityHelper.GetEntityByIdAsync(
            postsRepository.GetByIdAsync,
            request.PostsId,
            "Posts",
            logger);
        if (!posts.IsSuccess)
            return ResultT<IEnumerable<PostsWithCommunitiesDTos>>.Failure(posts.Error!);

        string cacheKey = $"get-posts-by-id-details-{request.PostsId}";
        var postsWithCommunities = await decoratedCache.GetOrCreateAsync(
            cacheKey,
            async () =>
            {
                var postsWithCommunity = await postsRepository.GetPostsByIdWithCommunitiesAsync(
                    request.PostsId,
                    cancellationToken);

                var postsWithCommunitiesDTos = postsWithCommunity.ToPostsWithCommunitiesDtos();

                return postsWithCommunitiesDTos;
            },
            cancellationToken: cancellationToken);

        IEnumerable<PostsWithCommunitiesDTos> postsWithCommunitiesDTosEnumerable = postsWithCommunities.ToList();
        if (!postsWithCommunitiesDTosEnumerable.Any())
        {
            logger.LogError("No communities found for post with ID '{PostsId}'.", request.PostsId);

            return ResultT<IEnumerable<PostsWithCommunitiesDTos>>.Failure(Error.Failure("400", "The list is empty"));
        }

        logger.LogInformation(
            "Successfully retrieved {Count} posts with their associated communities for post ID '{PostsId}'.",
            postsWithCommunitiesDTosEnumerable.Count(), request.PostsId);

        return ResultT<IEnumerable<PostsWithCommunitiesDTos>>.Success(postsWithCommunitiesDTosEnumerable);
    }
}