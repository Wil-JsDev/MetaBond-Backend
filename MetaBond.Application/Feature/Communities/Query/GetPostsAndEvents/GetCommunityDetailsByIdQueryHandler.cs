using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communities;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
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
    : IQueryHandler<GetCommunityDetailsByIdQuery, PagedResult<PostsAndEventsDTos>>
{
    public async Task<ResultT<PagedResult<PostsAndEventsDTos>>> Handle(
        GetCommunityDetailsByIdQuery request,
        CancellationToken cancellationToken)
    {
        var paginationValidate = PaginationHelper.ValidatePagination<PostsAndEventsDTos>
            (request.PageNumber, request.PageSize, logger);

        if (!paginationValidate.IsSuccess)
            return paginationValidate;

        var cacheKey = $"community-details-{request.Id}-page-{request.PageNumber}-size-{request.PageSize}";

        var result = await decoratedCache.GetOrCreateAsync(cacheKey, async () =>
        {
            var pagedCommunities = await communitiesRepository.GetPostsAndEventsByCommunityIdAsync(
                request.Id, request.PageNumber, request.PageSize, cancellationToken);

            var paged = pagedCommunities.Items!.Select(c =>
                CommunityMapper.ToDtos(c, c.Posts, c.Events));

            PagedResult<PostsAndEventsDTos> pagedResult = new(
                totalItems: pagedCommunities.TotalItems,
                currentPage: pagedCommunities.CurrentPage,
                items: paged,
                pageSize: request.PageSize
            );

            return pagedResult;
        }, cancellationToken: cancellationToken);

        if (!result.Items.Any())
        {
            logger.LogInformation("Community with ID {CommunityId} has no posts or events.", request.Id);

            return ResultT<PagedResult<PostsAndEventsDTos>>.Failure(
                Error.Failure("400", "Community has no posts or events."));
        }

        logger.LogInformation("Retrieved {Count} communities with posts and events for community {CommunityId}.",
            result.Items.Count(), request.Id);

        return ResultT<PagedResult<PostsAndEventsDTos>>.Success(result);
    }
}