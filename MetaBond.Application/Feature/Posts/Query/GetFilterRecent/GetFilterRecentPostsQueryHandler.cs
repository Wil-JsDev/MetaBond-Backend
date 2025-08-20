using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Posts;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Posts.Query.GetFilterRecent;

internal sealed class GetFilterRecentPostsQueryHandler(
    IPostsRepository postsRepository,
    IDistributedCache decoratedCache,
    ILogger<GetFilterRecentPostsQueryHandler> logger)
    : IQueryHandler<GetFilterRecentPostsQuery, IEnumerable<PostsDTos>>
{
    public async Task<ResultT<IEnumerable<PostsDTos>>> Handle(
        GetFilterRecentPostsQuery request,
        CancellationToken cancellationToken)
    {
        string cacheKey = $"get-filter-recent-top-count{request.TopCount}-community-{request.CommunitiesId}";
        var result = await decoratedCache.GetOrCreateAsync(
            cacheKey,
            async () =>
            {
                var postsFilters = await postsRepository.FilterRecentPostsByCountAsync(
                    request.CommunitiesId,
                    request.TopCount,
                    cancellationToken);

                IEnumerable<PostsDTos> postsDTos = postsFilters.Select(PostsMapper.PostsToDto);

                return postsDTos;
            },
            cancellationToken: cancellationToken);

        var postsDTosEnumerable = result.ToList();
        if (!postsDTosEnumerable.Any())
        {
            logger.LogError("No recent posts found with the specified count: {TopCount}", request.TopCount);

            return ResultT<IEnumerable<PostsDTos>>.Failure(Error.Failure("400", "The list is empty"));
        }

        logger.LogInformation("Successfully retrieved {Count} recent posts.", postsDTosEnumerable.Count());

        return ResultT<IEnumerable<PostsDTos>>.Success(postsDTosEnumerable);
    }
}