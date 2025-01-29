using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using MetaBond.Domain;

namespace MetaBond.Application.Feature.Friendship.Query.GetCountByStatus
{
    internal sealed class GetCountByStatusFriendshipQueryHandler : IQueryHandler<GetCountByStatusFriendshipQuery, int>
    {
        private readonly IFriendshipRepository _friendshipRepository;

        public GetCountByStatusFriendshipQueryHandler(IFriendshipRepository friendshipRepository)
        {
            _friendshipRepository = friendshipRepository;
        }

        public async Task<ResultT<int>> Handle(GetCountByStatusFriendshipQuery request, CancellationToken cancellationToken)
        {
            var friendship = CountStatusAsync();
            if (friendship.TryGetValue((request.Status), out var countFriendship))
            {
                var result = await countFriendship(cancellationToken);
                return ResultT<int>.Success(result);    
            }

            return ResultT<int>.Failure(Error.Failure("400", "Invalid status"));
        }

        private Dictionary<Domain.Status, Func<CancellationToken, Task<int>>> CountStatusAsync()
        {
            return new Dictionary<Domain.Status, Func<CancellationToken, Task<int>>>
            {
                {(Status.Accepted), async cancellationToken => await _friendshipRepository.CountByStatusAsync(Domain.Status.Accepted,cancellationToken)},
                {(Status.Pending), async cancellationToken => await _friendshipRepository.CountByStatusAsync(Domain.Status.Pending,cancellationToken)},
                {(Status.Blocked), async cancellationToken => await _friendshipRepository.CountByStatusAsync(Domain.Status.Blocked,cancellationToken)}
            };
        }
    }
}
