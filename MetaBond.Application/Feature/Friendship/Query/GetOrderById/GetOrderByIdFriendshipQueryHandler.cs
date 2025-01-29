using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;

namespace MetaBond.Application.Feature.Friendship.Query.GetOrderById
{
    internal sealed class GetOrderByIdFriendshipQueryHandler : IQueryHandler<GetOrderByIdFriendshipQuery, IEnumerable<FriendshipDTos>>
    {
        private readonly IFriendshipRepository _friendshipRepository;

        public GetOrderByIdFriendshipQueryHandler(IFriendshipRepository friendshipRepository)
        {
            _friendshipRepository = friendshipRepository;
        }

        public async Task<ResultT<IEnumerable<FriendshipDTos>>> Handle(GetOrderByIdFriendshipQuery request, CancellationToken cancellationToken)
        {

            var friendshipSort = GetSort();
            if (friendshipSort.TryGetValue((request.Sort.ToUpper()), out var GetSortFriendship))
            {
                var friendshipList = await GetSortFriendship(cancellationToken);
                if (friendshipList == null || !friendshipList.Any())
                {
                    return ResultT<IEnumerable<FriendshipDTos>>.Failure(Error.Failure("400", "The list is empty")); 
                }

                IEnumerable<FriendshipDTos> friendshipDTos = friendshipList.Select(x => new FriendshipDTos
                (
                    FriendshipId: x.Id,
                    Status: x.Status,
                    CreatedAt: x.CreateAt
                ));

                return ResultT<IEnumerable<FriendshipDTos>>.Success(friendshipDTos);

            }

            return ResultT<IEnumerable<FriendshipDTos>>.Failure
                (Error.Failure("400", "Invalid order parameter. Please specify 'asc' or 'desc'."));
        }

        private Dictionary<string, Func<CancellationToken, Task<IEnumerable<Domain.Models.Friendship>>>> GetSort()
        {
            return new Dictionary<string, Func<CancellationToken, Task<IEnumerable<Domain.Models.Friendship>>>>
            {
                {"asc", async cancellationToken => await _friendshipRepository.OrderByIdDescAsync(cancellationToken) },
                {"desc", async cancellationToken => await _friendshipRepository.OrderByIdDescAsync(cancellationToken) }
            };
        }


    }
}
