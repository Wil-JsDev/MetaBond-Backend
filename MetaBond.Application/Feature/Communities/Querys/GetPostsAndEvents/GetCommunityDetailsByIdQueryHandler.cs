using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communties;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;

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
            var communities = await _communitiesRepository.GetByIdAsync(request.Id);
            if (communities != null)
            {
                var communitiesWithEventsAndPosts = await _communitiesRepository.GetPostsAndEventsByCommunityIdAsync(request.Id,cancellationToken);

                var dTos = communitiesWithEventsAndPosts.Select(c => new PostsAndEventsDTos
                (
                    CommunitieId: c.Id,
                    Name: c.Name,
                    Category: c.Category,
                    CreatedAt: c.CreateAt,
                    Posts: c.Posts,
                    Events: c.Events
                ));

                return ResultT<IEnumerable<PostsAndEventsDTos>>.Success(dTos);
            }
            return ResultT<IEnumerable< PostsAndEventsDTos >>.Failure(Error.NotFound ("404","Community not found."));
        }
    }
}
