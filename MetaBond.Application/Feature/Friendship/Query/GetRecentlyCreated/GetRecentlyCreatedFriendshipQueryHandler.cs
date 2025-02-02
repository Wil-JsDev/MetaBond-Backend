using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Friendship.Query.GetRecentlyCreated
{
    internal sealed class GetRecentlyCreatedFriendshipQueryHandler : IQueryHandler<GetRecentlyCreatedFriendshipQuery, IEnumerable<FriendshipDTos>>
    {

        private readonly IFriendshipRepository _friendshipRepository;
        private readonly ILogger<GetRecentlyCreatedFriendshipQueryHandler> _logger;

        public GetRecentlyCreatedFriendshipQueryHandler(
            IFriendshipRepository friendshipRepository, 
            ILogger<GetRecentlyCreatedFriendshipQueryHandler> logger)
        {
            _friendshipRepository = friendshipRepository;
            _logger = logger;
        }

        public async Task<ResultT<IEnumerable<FriendshipDTos>>> Handle(
            GetRecentlyCreatedFriendshipQuery request, 
            CancellationToken cancellationToken)
        {
            if (request != null)
            {
               var friendshipRecently = await _friendshipRepository.GetRecentlyCreatedAsync(request.Limit,cancellationToken);
                if ( friendshipRecently == null || !friendshipRecently.Any())
                {
                    _logger.LogError("No recent friendships found for the given limit: {Limit}", request.Limit);

                    return ResultT<IEnumerable<FriendshipDTos>>.Failure(Error.Failure("400", "No recent friendships found."));
                }

                var friendshipDTos = friendshipRecently.Select(x => new FriendshipDTos
                (
                    FriendshipId: x.Id,
                    Status: x.Status,
                    CreatedAt: x.CreateAdt
                ));

                _logger.LogInformation("Successfully retrieved {Count} recent friendships with limit: {Limit}", 
                                       friendshipDTos.Count(), request.Limit);

                return ResultT<IEnumerable<FriendshipDTos>>.Success(friendshipDTos);

            }

            _logger.LogError("Request is null. Failed to retrieve recent friendships.");

            return ResultT<IEnumerable<FriendshipDTos>>.Failure(Error.Failure("400", "Invalid request: The request cannot be null."));
        }
    }
}
