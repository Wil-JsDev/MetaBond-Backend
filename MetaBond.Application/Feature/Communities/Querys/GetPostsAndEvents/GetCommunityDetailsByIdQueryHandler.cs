using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communties;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using MetaBond.Domain.Models;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Communities.Querys.GetPostsAndEvents
{
    internal sealed class GetCommunityDetailsByIdQueryHandler : IQueryHandler<GetCommunityDetailsByIdQuery, IEnumerable<PostsAndEventsDTos>>
    {
        private readonly ICommunitiesRepository _communitiesRepository;
        private readonly ILogger<GetCommunityDetailsByIdQueryHandler> _logger;
        private readonly IPostsRepository _postsRepository;
        private readonly IEventsRepository _eventsRepository;
        public GetCommunityDetailsByIdQueryHandler(
            ICommunitiesRepository communitiesRepository, 
            ILogger<GetCommunityDetailsByIdQueryHandler> logger,
            IPostsRepository postsRepository,
            IEventsRepository eventsRepository)
        {
            _communitiesRepository = communitiesRepository;
            _logger = logger;
            _postsRepository = postsRepository;
            _eventsRepository = eventsRepository;
        }

        public async Task<ResultT<IEnumerable<PostsAndEventsDTos>>> Handle(GetCommunityDetailsByIdQuery request, CancellationToken cancellationToken)
        {
            var community = await _communitiesRepository.GetByIdAsync(request.Id);
            if (community == null)
            {
                _logger.LogError("Community with ID {CommunityId} was not found.", request.Id);

                return ResultT<IEnumerable<PostsAndEventsDTos>>.Failure(Error.NotFound("404", "Community not found."));
            }

            var communitiesWithEventsAndPosts = await _communitiesRepository.GetPostsAndEventsByCommunityIdAsync(request.Id, cancellationToken);

            if (!communitiesWithEventsAndPosts.Any())
            {
                _logger.LogError("Community with ID {CommunityId} has no posts or events.", request.Id);

                return ResultT<IEnumerable<PostsAndEventsDTos>>.Failure(Error.NotFound("404", "No posts or events found for this community."));
            }

            if (request.PageNumber <= 0 || request.PageSize <= 0)
            {
                _logger.LogError("Invalid page number or page size. Both must be greater than zero.");
                
                return ResultT<IEnumerable<PostsAndEventsDTos>>.Failure(Error.Failure("400", "Page number and page size must be greater than zero."));
            }
            
            var postsPaged = await _postsRepository.GetPagedPostsAsync(request.PageNumber, request.PageSize,cancellationToken);
            var postsModel = postsPaged.Items.Select(x => new Domain.Models.Posts
            {
                Id = x.Id,
                Title = x.Title,
                Content = x.Content,
                Image = x.Image,
                CommunitiesId = x.CommunitiesId,
                CreatedAt = x.CreatedAt
            }).ToList();
            
            var eventsPaged = await _eventsRepository.GetPagedEventsAsync(request.PageNumber, request.PageSize,cancellationToken);
            var eventsModel = eventsPaged.Items.Select(x => new Domain.Models.Events
            {
                Id = x.Id,
                Description = x.Description,
                Title = x.Title,
                DateAndTime = x.DateAndTime,
                CreateAt = x.CreateAt,
                CommunitiesId = x.CommunitiesId
            }).ToList();
            
            var dTos = communitiesWithEventsAndPosts.Select(c => new PostsAndEventsDTos
            (
                CommunitieId: c.Id,
                Name: c.Name,
                Category: c.Category,
                CreatedAt: c.CreateAt,
                Posts: postsModel, 
                Events: eventsModel
            ));
            
            _logger.LogInformation("Successfully retrieved posts and events for community with ID {CommunityId}.", request.Id);
            
            return ResultT<IEnumerable<PostsAndEventsDTos>>.Success(dTos);

        }
    }
}
