using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Friendship.Query.GetFilterByStatus
{
    internal sealed class FilterByStatusFriendshipQueryHandler : IQueryHandler<FilterByStatusFriendshipQuery, IEnumerable<FriendshipDTos>>
    {
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly ILogger<FilterByStatusFriendshipQueryHandler> _logger;

        public FilterByStatusFriendshipQueryHandler(
            IFriendshipRepository friendshipRepository, 
            ILogger<FilterByStatusFriendshipQueryHandler> logger)
        {
            _friendshipRepository = friendshipRepository;
            _logger = logger;
        }

        public async Task<ResultT<IEnumerable<FriendshipDTos>>> Handle(
            FilterByStatusFriendshipQuery request, 
            CancellationToken cancellationToken)
        {
            var exists = await _friendshipRepository.ValidateAsync(x => x.Status == request.Status);
            if (!exists)
            {
                _logger.LogError("No active friendship found with status '{Status}'.", request.Status);
    
                return ResultT<IEnumerable<FriendshipDTos>>.Failure(Error.NotFound("404", $"No active friendship exists with status '{request.Status}'.")); 
            }
            var getStatusFriendship = GetStatusFriendship();
            if (getStatusFriendship.TryGetValue((request.Status), out var statusFilter))
            {
                var friendship = await statusFilter(cancellationToken);
                if (friendship == null || !friendship.Any())
                {
                    _logger.LogError("No friendships found with status: {Status}", request.Status);

                    return ResultT<IEnumerable<FriendshipDTos>>.Failure(Error.Failure("400", "No friendships found with the given status"));
                }

                IEnumerable<FriendshipDTos> friendshipDTos = friendship.Select(x => new FriendshipDTos
                (
                    FriendshipId: x.Id,
                    Status: x.Status,
                    CreatedAt: x.CreateAdt
                ));

                _logger.LogInformation("Successfully retrieved {Count} friendships with status: {Status}", 
                                        friendshipDTos.Count(), request.Status);

                return ResultT<IEnumerable<FriendshipDTos>>.Success(friendshipDTos);
            }

            _logger.LogError("Failed to retrieve friendships: Invalid status {Status}", request.Status);

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
