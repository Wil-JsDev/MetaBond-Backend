using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Friendship.Query.GetFilterByStatus;

internal sealed class FilterByStatusFriendshipQueryHandler(
    IFriendshipRepository friendshipRepository,
    IDistributedCache decoratedCache,
    ILogger<FilterByStatusFriendshipQueryHandler> logger)
    : IQueryHandler<FilterByStatusFriendshipQuery, IEnumerable<FriendshipDTos>>
{
    public async Task<ResultT<IEnumerable<FriendshipDTos>>> Handle(
        FilterByStatusFriendshipQuery request, 
        CancellationToken cancellationToken)
    {
        var exists = await friendshipRepository.ValidateAsync(x => x.Status == request.Status);
        if (!exists)
        {
            logger.LogError("No active friendship found with status '{Status}'.", request.Status);
    
            return ResultT<IEnumerable<FriendshipDTos>>.Failure(Error.NotFound("404", $"No active friendship exists with status '{request.Status}'.")); 
        }
        
        var getStatusFriendship = GetStatusFriendship();
        if (getStatusFriendship.TryGetValue((request.Status), out var statusFilter))
        {
            string cacheKey = $"FilterStatusFriendship-{request.Status}";
            var friendshipStatusFilter = await decoratedCache.GetOrCreateAsync(
                cacheKey,
                async () => await statusFilter(cancellationToken), 
                cancellationToken: cancellationToken);
            IEnumerable<Domain.Models.Friendship> friendships = friendshipStatusFilter.ToList();
            if (!friendships.Any())
            {
                logger.LogError("No friendships found with status: {Status}", request.Status);

                return ResultT<IEnumerable<FriendshipDTos>>.Failure(Error.Failure("400", "No friendships found with the given status"));
            }

            IEnumerable<FriendshipDTos> friendshipDTos = friendships.Select(x => new FriendshipDTos
            (
                FriendshipId: x.Id,
                Status: x.Status,
                CreatedAt: x.CreateAdt
            ));

            IEnumerable<FriendshipDTos> friendshipDTosEnumerable = friendshipDTos.ToList();
            logger.LogInformation("Successfully retrieved {Count} friendships with status: {Status}", 
                friendshipDTosEnumerable.Count(), request.Status);

            return ResultT<IEnumerable<FriendshipDTos>>.Success(friendshipDTosEnumerable);
        }

        logger.LogError("Failed to retrieve friendships: Invalid status {Status}", request.Status);

        return ResultT<IEnumerable<FriendshipDTos>>.Failure(Error.Failure("400", "Invalid status"));
    }

    #region Private Methods
    private Dictionary<Status, Func<CancellationToken, Task<IEnumerable<Domain.Models.Friendship>>>> GetStatusFriendship()
    {
        return new Dictionary<Status, Func<CancellationToken, Task<IEnumerable<Domain.Models.Friendship>>>>()
        {
            { (Status.Pending), async cancellationToken => await friendshipRepository.GetFilterByStatusAsync (Status.Pending,cancellationToken)},
            {(Status.Accepted), async cancellationToken => await friendshipRepository.GetFilterByStatusAsync (Status.Accepted,cancellationToken)},
            {(Status.Blocked), async cancellationToken => await friendshipRepository.GetFilterByStatusAsync (Status.Blocked,cancellationToken)}
        };
    }
    #endregion
}