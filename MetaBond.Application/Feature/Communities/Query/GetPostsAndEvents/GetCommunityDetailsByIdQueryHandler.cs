using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communities;
using MetaBond.Application.Feature.Communities.Query.GetPostsAndEvents;
using MetaBond.Application.Interfaces.Repository;
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
        public async Task<ResultT<IEnumerable<PostsAndEventsDTos>>> Handle(GetCommunityDetailsByIdQuery request, CancellationToken cancellationToken)
        {
            var community = await communitiesRepository.GetByIdAsync(request.Id);
            if (community == null)
            {
                logger.LogError("Community with ID {CommunityId} was not found.", request.Id);

                return ResultT<IEnumerable<PostsAndEventsDTos>>.Failure(Error.NotFound("404", "Community not found."));
            }

            var communitiesWithEventsAndPosts = await decoratedCache.GetOrCreateAsync($"communities-{community.Id}",
                async () => await communitiesRepository.GetPostsAndEventsByCommunityIdAsync(request.Id, cancellationToken), cancellationToken: cancellationToken);

            IEnumerable<Domain.Models.Communities> withEventsAndPosts = communitiesWithEventsAndPosts.ToList();

            if (!withEventsAndPosts.Any())
            {
                logger.LogError("Community with ID {CommunityId} has no posts or events.", request.Id);

                return ResultT<IEnumerable<PostsAndEventsDTos>>.Failure(Error.NotFound("404", "No posts or events found for this community."));
            }

            if (request.PageNumber <= 0 || request.PageSize <= 0)
            {
                logger.LogError("Invalid page number or page size. Both must be greater than zero.");
                
                return ResultT<IEnumerable<PostsAndEventsDTos>>.Failure(Error.Failure("400", "Page number and page size must be greater than zero."));
            }
            
            var postsPaged = await postsRepository.GetPagedPostsAsync(request.PageNumber, request.PageSize,cancellationToken);
            var postsModel = postsPaged.Items!.Select(x => new Domain.Models.Posts
            {
                Id = x.Id,
                Title = x.Title,
                Content = x.Content,
                Image = x.Image,
                CommunitiesId = x.CommunitiesId,
                CreatedAt = x.CreatedAt
            }).ToList();
            
            var eventsPaged = await eventsRepository.GetPagedEventsAsync(request.PageNumber, request.PageSize,cancellationToken);
            var eventsModel = eventsPaged.Items!.Select(x => new Domain.Models.Events
            {
                Id = x.Id,
                Description = x.Description,
                Title = x.Title,
                DateAndTime = x.DateAndTime,
                CreateAt = x.CreateAt,
                CommunitiesId = x.CommunitiesId
            }).ToList();
            
            var dTos = withEventsAndPosts.Select(c => new PostsAndEventsDTos
            (
                CommunitieId: c.Id,
                Name: c.Name,
                Category: c.Category,
                CreatedAt: c.CreateAt,
                Posts: postsModel, 
                Events: eventsModel
            ));
            
            logger.LogInformation("Successfully retrieved posts and events for community with ID {CommunityId}.", request.Id);
            
            return ResultT<IEnumerable<PostsAndEventsDTos>>.Success(dTos);

        }
    }
