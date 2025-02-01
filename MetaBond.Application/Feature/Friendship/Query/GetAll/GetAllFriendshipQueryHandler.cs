using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Friendship.Query.GetAll
{
    internal sealed class GetAllFriendshipQueryHandler : IQueryHandler<GetAllFriendshipQuery, IEnumerable<FriendshipDTos>>
    {
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly ILogger<GetAllFriendshipQueryHandler> _logger;

        public GetAllFriendshipQueryHandler(
            IFriendshipRepository friendshipRepository,
            ILogger<GetAllFriendshipQueryHandler> logger)
        {
            _friendshipRepository = friendshipRepository;
            _logger = logger;
        }

        public async Task<ResultT<IEnumerable<FriendshipDTos>>> Handle(
            GetAllFriendshipQuery request, 
            CancellationToken cancellationToken)
        {
            var friendship = await _friendshipRepository.GetAll(cancellationToken);
            if (!friendship.Any())
            {
                _logger.LogError("No friendships found in the database.");

                return ResultT<IEnumerable<FriendshipDTos>>.Failure(Error.Failure("400", "No recent friendships found")); 
            }

            IEnumerable<FriendshipDTos> friendshipDTos = friendship.Select(x => new FriendshipDTos
            (
                FriendshipId: x.Id,
                Status: x.Status,
                CreatedAt: x.CreateAdt
            ));

            _logger.LogInformation("Retrieved {Count} friendships successfully.", friendshipDTos.Count());

            return ResultT<IEnumerable<FriendshipDTos>>.Success(friendshipDTos);
        }
    }
}
