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

        public GetCommunityDetailsByIdQueryHandler(ICommunitiesRepository communitiesRepository, ILogger<GetCommunityDetailsByIdQueryHandler> logger)
        {
            _communitiesRepository = communitiesRepository;
            _logger = logger;
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

            var dTos = communitiesWithEventsAndPosts.Select(c => new PostsAndEventsDTos
            (
                CommunitieId: c.Id,
                Name: c.Name,
                Category: c.Category,
                CreatedAt: c.CreateAt,
                Posts: c.Posts ?? new List<Domain.Models.Posts>(), 
                Events: c.Events ?? new List<Domain.Models.Events>() 
            ));

            _logger.LogInformation("Successfully retrieved posts and events for community with ID {CommunityId}.", request.Id);
            return ResultT<IEnumerable<PostsAndEventsDTos>>.Success(dTos);

        }
    }
}
