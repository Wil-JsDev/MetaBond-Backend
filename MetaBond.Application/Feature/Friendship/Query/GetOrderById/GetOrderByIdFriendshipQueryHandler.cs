using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Friendship.Query.GetOrderById;

internal sealed class GetOrderByIdFriendshipQueryHandler(
    IFriendshipRepository friendshipRepository,
    ILogger<GetOrderByIdFriendshipQueryHandler> logger)
    : IQueryHandler<GetOrderByIdFriendshipQuery, IEnumerable<FriendshipDTos>>
{
    public async Task<ResultT<IEnumerable<FriendshipDTos>>> Handle(
        GetOrderByIdFriendshipQuery request, 
        CancellationToken cancellationToken)
    {

        var friendshipSort = GetSort();
        if (friendshipSort.TryGetValue((request.Sort!.ToUpper()), out var getSortFriendship))
        {
            var friendshipList = await getSortFriendship(cancellationToken);
            IEnumerable<Domain.Models.Friendship> friendships = friendshipList.ToList();
            if (friendshipList == null || !friendships.Any())
            {
                logger.LogError("No friendships found for the sorted order: {Sort}", request.Sort);

                return ResultT<IEnumerable<FriendshipDTos>>.Failure(Error.Failure("400", "The list is empty"));
            }

            IEnumerable<FriendshipDTos> friendshipDTos = friendships.Select(x => new FriendshipDTos
            (
                FriendshipId: x.Id,
                Status: x.Status,
                CreatedAt: x.CreateAdt
            ));

            IEnumerable<FriendshipDTos> dTosEnumerable = friendshipDTos.ToList();
            logger.LogInformation("Successfully retrieved {Count} friendships sorted by: {Sort}", 
                dTosEnumerable.Count(), request.Sort);

            return ResultT<IEnumerable<FriendshipDTos>>.Success(dTosEnumerable);
        }

        logger.LogError("Invalid order parameter: {Sort}. Expected 'asc' or 'desc'.", request.Sort);

        return ResultT<IEnumerable<FriendshipDTos>>.Failure
            (Error.Failure("400", "Invalid order parameter. Please specify 'asc' or 'desc'."));
    }

    #region Private Methods
    private Dictionary<string, Func<CancellationToken, Task<IEnumerable<Domain.Models.Friendship>>>> GetSort()
    {
        return new Dictionary<string, Func<CancellationToken, Task<IEnumerable<Domain.Models.Friendship>>>>
        {
            {"asc", async cancellationToken => await friendshipRepository.OrderByIdAscAsync(cancellationToken) },
            {"desc", async cancellationToken => await friendshipRepository.OrderByIdDescAsync(cancellationToken) }
        };
    }
    #endregion
}