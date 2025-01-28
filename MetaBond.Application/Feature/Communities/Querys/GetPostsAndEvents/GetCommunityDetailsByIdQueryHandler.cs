using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communties;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Feature.Communities.Querys.GetPostsAndEvents
{
    internal sealed class GetCommunityDetailsByIdQueryHandler : IQueryHandler<GetCommunityDetailsByIdQuery, IEnumerable<PostsAndEventsDTos>>
    {
        private readonly ICommunitiesRepository _communitiesRepository;

        public GetCommunityDetailsByIdQueryHandler(ICommunitiesRepository communitiesRepository)
        {
            _communitiesRepository = communitiesRepository;
        }

        public async Task<ResultT<IEnumerable<PostsAndEventsDTos>>> Handle(GetCommunityDetailsByIdQuery request, CancellationToken cancellationToken)
        {
            var community = await _communitiesRepository.GetByIdAsync(request.Id);
            if (community == null)
            {
                return ResultT<IEnumerable<PostsAndEventsDTos>>.Failure(Error.NotFound("404", "Community not found."));
            }

            var communitiesWithEventsAndPosts = await _communitiesRepository.GetPostsAndEventsByCommunityIdAsync(request.Id, cancellationToken);

            if (!communitiesWithEventsAndPosts.Any())
            {
                return ResultT<IEnumerable<PostsAndEventsDTos>>.Failure(Error.NotFound("404", "No posts or events found for this community."));
            }

            var dTos = communitiesWithEventsAndPosts.Select(c => new PostsAndEventsDTos
            (
                CommunitieId: c.Id,
                Name: c.Name,
                Category: c.Category,
                CreatedAt: c.CreateAt,
                Posts: c.Posts ?? new List<Posts>(), 
                Events: c.Events ?? new List<Domain.Models.Events>() 
            ));

            return ResultT<IEnumerable<PostsAndEventsDTos>>.Success(dTos);
        }
    }
}
