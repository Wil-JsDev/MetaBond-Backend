using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Posts;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Posts.Query.GetFilterTop10;

internal sealed class GetFilterTop10QueryHandler(
    IPostsRepository postsRepository,
    IDistributedCache decoratedCache,
    ILogger<GetFilterTop10QueryHandler> logger)
    : IQueryHandler<GetFilterTop10Query, IEnumerable<PostsDTos>>
{
    public async Task<ResultT<IEnumerable<PostsDTos>>> Handle(
        GetFilterTop10Query request,
        CancellationToken cancellationToken)
    {
        string cacheKey = $"top-count-recent-posts-{request.CommunitiesId}";
        var top10CountPosts = await decoratedCache.GetOrCreateAsync(
            cacheKey,
            async () =>
            {
                var top10CountPosts = await postsRepository.FilterTop10RecentPostsAsync(request.CommunitiesId,
                    cancellationToken);

                IEnumerable<PostsDTos> postsDTos = top10CountPosts.Select(PostsMapper.PostsToDto);

                return postsDTos;
            },
            cancellationToken: cancellationToken);

        var postsDTosEnumerable = top10CountPosts.ToList();
        if (!postsDTosEnumerable.Any())
        {
            logger.LogError("No posts available in the top 10 recent posts list.");

            return ResultT<IEnumerable<PostsDTos>>.Failure(Error.Failure("400", "The list is empty"));
        }

        logger.LogInformation("Successfully retrieved {Count} recent posts.", postsDTosEnumerable.Count());

        return ResultT<IEnumerable<PostsDTos>>.Success(postsDTosEnumerable);
    }
}