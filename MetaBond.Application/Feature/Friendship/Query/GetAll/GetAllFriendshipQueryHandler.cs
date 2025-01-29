using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;

namespace MetaBond.Application.Feature.Friendship.Query.GetAll
{
    internal sealed class GetAllFriendshipQueryHandler : IQueryHandler<GetAllFriendshipQuery, IEnumerable<FriendshipDTos>>
    {
        private readonly IFriendshipRepository _friendshipRepository;
            
        public GetAllFriendshipQueryHandler(IFriendshipRepository friendshipRepository)
        {
            _friendshipRepository = friendshipRepository;
        }

        public async Task<ResultT<IEnumerable<FriendshipDTos>>> Handle(GetAllFriendshipQuery request, CancellationToken cancellationToken)
        {
            var friendship = await _friendshipRepository.GetAll(cancellationToken);
            if (!friendship.Any())
            {
                return ResultT<IEnumerable<FriendshipDTos>>.Failure(Error.Failure("400", "No recent friendships found"));
            }
            IEnumerable<FriendshipDTos> friendshipDTos = friendship.Select(x => new FriendshipDTos
            (
                FriendshipId: x.Id,
                Status: x.Status,
                CreatedAt: x.CreateAt
            ));

            return ResultT<IEnumerable<FriendshipDTos>>.Success(friendshipDTos);
        }
    }
}
