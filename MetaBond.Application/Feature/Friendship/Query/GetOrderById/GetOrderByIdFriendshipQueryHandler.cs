using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Friendship.Query.GetOrderById
{
    internal sealed class GetOrderByIdFriendshipQueryHandler : IQueryHandler<GetOrderByIdFriendshipQuery, IEnumerable<FriendshipDTos>>
    {
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly ILogger<GetOrderByIdFriendshipQueryHandler> _logger;

        public GetOrderByIdFriendshipQueryHandler(
            IFriendshipRepository friendshipRepository, 
            ILogger<GetOrderByIdFriendshipQueryHandler> logger)
        {
            _friendshipRepository = friendshipRepository;
            _logger = logger;
        }

        public async Task<ResultT<IEnumerable<FriendshipDTos>>> Handle(
            GetOrderByIdFriendshipQuery request, 
            CancellationToken cancellationToken)
        {

            var friendshipSort = GetSort();
            if (friendshipSort.TryGetValue((request.Sort.ToUpper()), out var GetSortFriendship))
            {
                var friendshipList = await GetSortFriendship(cancellationToken);
                if (friendshipList == null || !friendshipList.Any())
                {
                    _logger.LogError("No friendships found for the sorted order: {Sort}", request.Sort);

                    return ResultT<IEnumerable<FriendshipDTos>>.Failure(Error.Failure("400", "The list is empty"));
                }

                IEnumerable<FriendshipDTos> friendshipDTos = friendshipList.Select(x => new FriendshipDTos
                (
                    FriendshipId: x.Id,
                    Status: x.Status,
                    CreatedAt: x.CreateAdt
                ));

                _logger.LogInformation("Successfully retrieved {Count} friendships sorted by: {Sort}", 
                                        friendshipDTos.Count(), request.Sort);

                return ResultT<IEnumerable<FriendshipDTos>>.Success(friendshipDTos);
            }

            _logger.LogError("Invalid order parameter: {Sort}. Expected 'asc' or 'desc'.", request.Sort);

            return ResultT<IEnumerable<FriendshipDTos>>.Failure
                (Error.Failure("400", "Invalid order parameter. Please specify 'asc' or 'desc'."));
        }

        private Dictionary<string, Func<CancellationToken, Task<IEnumerable<Domain.Models.Friendship>>>> GetSort()
        {
            return new Dictionary<string, Func<CancellationToken, Task<IEnumerable<Domain.Models.Friendship>>>>
            {
                {"asc", async cancellationToken => await _friendshipRepository.OrderByIdAscAsync(cancellationToken) },
                {"desc", async cancellationToken => await _friendshipRepository.OrderByIdDescAsync(cancellationToken) }
            };
        }


    }
}
