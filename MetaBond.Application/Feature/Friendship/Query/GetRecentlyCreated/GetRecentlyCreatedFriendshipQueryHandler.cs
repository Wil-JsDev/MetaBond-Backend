using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;

namespace MetaBond.Application.Feature.Friendship.Query.GetRecentlyCreated
{
    internal sealed class GetRecentlyCreatedFriendshipQueryHandler : IQueryHandler<GetRecentlyCreatedFriendshipQuery, IEnumerable<FriendshipDTos>>
    {

        private readonly IFriendshipRepository _friendshipRepository;

        public GetRecentlyCreatedFriendshipQueryHandler(IFriendshipRepository friendshipRepository)
        {
            _friendshipRepository = friendshipRepository;
        }

        public async Task<ResultT<IEnumerable<FriendshipDTos>>> Handle(GetRecentlyCreatedFriendshipQuery request, CancellationToken cancellationToken)
        {
            if (request != null)
            {
               var friendshipRecently = await _friendshipRepository.GetRecentlyCreatedAsync(request.Limit,cancellationToken);
                if ( friendshipRecently == null || !friendshipRecently.Any())
                {
                    return ResultT<IEnumerable<FriendshipDTos>>.Failure(Error.Failure("400", "No recent friendships found."));
                }

                var friendshipDTos = friendshipRecently.Select(x => new FriendshipDTos
                (
                    FriendshipId: x.Id,
                    Status: x.Status,
                    CreatedAt: x.CreateAt
                ));

                return ResultT<IEnumerable<FriendshipDTos>>.Success(friendshipDTos);

            }

            return ResultT<IEnumerable<FriendshipDTos>>.Failure(Error.Failure("400", "Invalid request: The request cannot be null."));

        }
    }
}
