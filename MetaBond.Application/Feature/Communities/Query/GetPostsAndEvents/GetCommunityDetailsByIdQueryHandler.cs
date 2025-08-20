using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communities;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using MetaBond.Domain.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Communities.Query.GetPostsAndEvents;

internal sealed class GetCommunityDetailsByIdQueryHandler(
    ICommunitiesRepository communitiesRepository,
    ILogger<GetCommunityDetailsByIdQueryHandler> logger,
    IDistributedCache decoratedCache,
    IPostsRepository postsRepository,
    IEventsRepository eventsRepository)
    : IQueryHandler<GetCommunityDetailsByIdQuery, IEnumerable<PostsAndEventsDTos>>
{
    public async Task<ResultT<IEnumerable<PostsAndEventsDTos>>> Handle(GetCommunityDetailsByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (request.PageNumber <= 0 || request.PageSize <= 0)
        {
            logger.LogError("Invalid page number or page size. Both must be greater than zero.");

            return ResultT<IEnumerable<PostsAndEventsDTos>>.Failure(Error.Failure("400",
                "Page number and page size must be greater than zero."));
        }

        var community = await communitiesRepository.GetByIdAsync(request.Id);
        if (community == null)
        {
            logger.LogError("Community with ID {CommunityId} was not found.", request.Id);

            return ResultT<IEnumerable<PostsAndEventsDTos>>.Failure(Error.NotFound("404", "Community not found."));
        }

        var cacheKey = $"community-details-{request.Id}-page-{request.PageNumber}-size-{request.PageSize}";

        var result = await decoratedCache.GetOrCreateAsync(cacheKey, async () =>
        {
            var postsTask = postsRepository.GetPagedPostsAsync(request.PageNumber, request.PageSize, cancellationToken);
            var eventsTask = eventsRepository.GetPagedEventsAsync(request.PageNumber, request.PageSize, cancellationToken);

            await Task.WhenAll(postsTask, eventsTask);

            return new List<PostsAndEventsDTos>
            {
                CommunityMapper.ToDtos(
                    community,
                    postsTask.Result.Items,
                    eventsTask.Result.Items
                )
            };
        }, cancellationToken: cancellationToken);

        if (!result.Any())
        {
            logger.LogInformation("Community with ID {CommunityId} has no posts or events.", request.Id);

            return ResultT<IEnumerable<PostsAndEventsDTos>>.Failure(Error.Failure("400",
                "Community has no posts or events."));
        }

        logger.LogInformation("Retrieved {Count} posts and events for community {CommunityId}.",
            result.Count(), request.Id);

        return ResultT<IEnumerable<PostsAndEventsDTos>>.Success(result);
    }
}