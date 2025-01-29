using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using MetaBond.Domain;

namespace MetaBond.Application.Feature.Friendship.Query.GetFilterByStatus
{
    internal sealed class FilterByStatusFriendshipQueryHandler : IQueryHandler<FilterByStatusFriendshipQuery, IEnumerable<FriendshipDTos>>
    {
        private readonly IFriendshipRepository _friendshipRepository;

        public FilterByStatusFriendshipQueryHandler(IFriendshipRepository friendshipRepository)
        {
            _friendshipRepository = friendshipRepository;
        }

        public async Task<ResultT<IEnumerable<FriendshipDTos>>> Handle(FilterByStatusFriendshipQuery request, CancellationToken cancellationToken)
        {
            var getStatusFriendship = GetStatusFriendship();

            if (getStatusFriendship.TryGetValue((request.Status), out var statusFilter))
            {
                var friendship = await statusFilter(cancellationToken);
                IEnumerable<FriendshipDTos> friendshipDTos = friendship.Select(x => new FriendshipDTos
                (
                    FriendshipId: x.Id,
                    Status: x.Status,
                    CreatedAt: x.CreateAt
                ));

                return ResultT<IEnumerable<FriendshipDTos>>.Success(friendshipDTos);
            }

            return ResultT<IEnumerable<FriendshipDTos>>.Failure(Error.Failure("400", "Invalid status"));

        }

        private Dictionary<Status, Func<CancellationToken, Task<IEnumerable<Domain.Models.Friendship>>>> GetStatusFriendship()
        {
            return new Dictionary<Status, Func<CancellationToken, Task<IEnumerable<Domain.Models.Friendship>>>>()
            {
                { (Status.Pending), async cancellationToken => await _friendshipRepository.GetFilterByStatusAsync (Status.Pending,cancellationToken)},
                {(Status.Accepted), async cancellationToken => await _friendshipRepository.GetFilterByStatusAsync (Status.Accepted,cancellationToken)},
                {(Status.Blocked), async cancellationToken => await _friendshipRepository.GetFilterByStatusAsync (Status.Blocked,cancellationToken)}
            };
        }

    }
}
