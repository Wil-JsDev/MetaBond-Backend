using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Friendship.Query.GetCountByStatus;

internal sealed class GetCountByStatusFriendshipQueryHandler(
    IFriendshipRepository friendshipRepository,
    ILogger<GetCountByStatusFriendshipQueryHandler> logger)
    : IQueryHandler<GetCountByStatusFriendshipQuery, int>
{
    public async Task<ResultT<int>> Handle(
        GetCountByStatusFriendshipQuery request, 
        CancellationToken cancellationToken)
    {
        var friendship = CountStatusAsync();
        if (friendship.TryGetValue((request.Status), out var countFriendship))
        {
            var result = await countFriendship(cancellationToken);

            logger.LogInformation("Successfully retrieved count for status {Status}: {Count}", request.Status, result);

            return ResultT<int>.Success(result);    
        }
        logger.LogError("Failed to retrieve count: Invalid status {Status}", request.Status);

        return ResultT<int>.Failure(Error.Failure("400", "Invalid status"));
    }

    #region Private Methods
    private Dictionary<Domain.Status, Func<CancellationToken, Task<int>>> CountStatusAsync()
    {
        return new Dictionary<Domain.Status, Func<CancellationToken, Task<int>>>
        {
            {(Status.Accepted), async cancellationToken => await friendshipRepository.CountByStatusAsync(Domain.Status.Accepted,cancellationToken)},
            {(Status.Pending), async cancellationToken => await friendshipRepository.CountByStatusAsync(Domain.Status.Pending,cancellationToken)},
            {(Status.Blocked), async cancellationToken => await friendshipRepository.CountByStatusAsync(Domain.Status.Blocked,cancellationToken)}
        };
    }
    #endregion
}