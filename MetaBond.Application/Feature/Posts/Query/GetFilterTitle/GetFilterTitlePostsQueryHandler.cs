using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Posts;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Posts.Query.GetFilterTitle;

internal sealed class GetFilterTitlePostsQueryHandler(
    IPostsRepository postsRepository,
    IDistributedCache decoratedCache,
    ILogger<GetFilterTitlePostsQueryHandler> logger)
    : IQueryHandler<GetFilterTitlePostsQuery, IEnumerable<PostsDTos>>
{
    public async Task<ResultT<IEnumerable<PostsDTos>>> Handle(
        GetFilterTitlePostsQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Title))
        {
            logger.LogError("Invalid request: GetFilterTitlePostsQuery request is null.");

            return ResultT<IEnumerable<PostsDTos>>.Failure(Error.Failure("400", "Invalid request"));
        }

        var exists = await postsRepository.ValidateAsync(x => x.Title == request.Title, cancellationToken);
        if (!exists)
        {
            logger.LogError("No post found with the title '{Title}'.", request.Title);

            return ResultT<IEnumerable<PostsDTos>>.Failure(Error.NotFound("404",
                $"No post exists with the title '{request.Title}'"));
        }

        string cacheKey = $"community-filter-title-{request.CommunitiesId}-{request.Title}";
        var result = await decoratedCache.GetOrCreateAsync(
            cacheKey,
            async () =>
            {
                var postsWithTitle = await postsRepository.GetFilterByTitleAsync(
                    request.CommunitiesId,
                    request.Title,
                    cancellationToken);

                var postsDTos = postsWithTitle.Select(PostsMapper.PostsToDto);

                return postsDTos;
            },
            cancellationToken: cancellationToken);

        IEnumerable<PostsDTos> postsDTosEnumerable = result.ToList();
        if (!postsDTosEnumerable.Any())
        {
            logger.LogError("No posts found with the title '{Title}'.", request.Title);

            return ResultT<IEnumerable<PostsDTos>>.Failure(Error.Failure("400", "The list is empty"));
        }

        logger.LogInformation("Successfully retrieved {Count} posts with the title '{Title}'.",
            postsDTosEnumerable.Count(), request.Title);

        return ResultT<IEnumerable<PostsDTos>>.Success(postsDTosEnumerable);
    }
}