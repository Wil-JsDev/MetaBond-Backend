using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.DTOs.Posts;
using MetaBond.Application.Interfaces.Repository;
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
        var posts = await postsRepository.GetByIdAsync(request.PostsId);
        if (posts == null)
        {
            logger.LogError("Post with ID '{PostsId}' not found.", request.PostsId);

            return ResultT<IEnumerable<PostsWithCommunitiesDTos>>.Failure(Error.NotFound("404", $"{request.PostsId} not found"));
        }

        string cacheKey = $"get-posts-by-id-details-{request.PostsId}";
        var postsWithCommunities = await decoratedCache.GetOrCreateAsync(
            cacheKey,
            async () => await postsRepository.GetPostsByIdWithCommunitiesAsync(
                request.PostsId, 
                cancellationToken), 
            cancellationToken: cancellationToken);
        
        IEnumerable<Domain.Models.Posts> withCommunities = postsWithCommunities.ToList();
        if (!withCommunities.Any())
        {
            logger.LogError("No communities found for post with ID '{PostsId}'.", request.PostsId);

            return ResultT<IEnumerable<PostsWithCommunitiesDTos>>.Failure(Error.Failure("400","The list is empty"));
        }

        IEnumerable<PostsWithCommunitiesDTos> postsWithCommunitiesDTos = withCommunities.Select(x => new PostsWithCommunitiesDTos
        (
            PostsId: x.Id,
            Title: x.Title,
            Content: x.Content,
            ImageUrl: x.Image,
            Communities: x.Communities != null 
                ?
                [
                    new CommunitySummaryDto(
                        x.Communities.Description,
                        x.Communities.Category,
                        x.Communities.CreateAt
                    )
                ] : new List<CommunitySummaryDto>(),
            CreatedAt: x.CreatedAt
        ));

        IEnumerable<PostsWithCommunitiesDTos> postsWithCommunitiesDTosEnumerable = postsWithCommunitiesDTos.ToList();
        logger.LogInformation("Successfully retrieved {Count} posts with their associated communities for post ID '{PostsId}'.", 
            postsWithCommunitiesDTosEnumerable.Count(), request.PostsId);

        return ResultT<IEnumerable<PostsWithCommunitiesDTos>>.Success(postsWithCommunitiesDTosEnumerable);
    }
}