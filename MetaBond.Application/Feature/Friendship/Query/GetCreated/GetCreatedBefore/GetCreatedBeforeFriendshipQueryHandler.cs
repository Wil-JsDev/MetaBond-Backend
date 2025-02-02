using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Friendship.Query.GetCreated.GetCreatedBefore
{
    internal sealed class GetCreatedBeforeFriendshipQueryHandler : IQueryHandler<GetCreatedBeforeFriendshipQuery, IEnumerable<FriendshipDTos>>
    {

        private readonly IFriendshipRepository _friendshipRepository;
        private readonly ILogger<GetCreatedBeforeFriendshipQueryHandler> _logger;

        public GetCreatedBeforeFriendshipQueryHandler(
            IFriendshipRepository friendshipRepository, 
            ILogger<GetCreatedBeforeFriendshipQueryHandler> logger)
        {
            _friendshipRepository = friendshipRepository;
            _logger = logger;
        }

        public async Task<ResultT<IEnumerable<FriendshipDTos>>> Handle(
            GetCreatedBeforeFriendshipQuery request, 
            CancellationToken cancellationToken)
        {

            var friendshipBefore = GetCreatedBefore();

            if (friendshipBefore.TryGetValue((request.PastDateRangeType), out var beforeAsync))
            {
                var friendshipList = await beforeAsync(cancellationToken);
                if (friendshipList == null || !friendshipList.Any())
                {
                    _logger.LogError("No friendships found before the specified date range: {PastDateRangeType}", request.PastDateRangeType);

                    return ResultT<IEnumerable<FriendshipDTos>>.Failure(Error.Failure("400", "The list is empty"));
                }

                var friendshipDTos = friendshipList.Select(x => new FriendshipDTos
                (
                    FriendshipId: x.Id,
                    Status: x.Status,
                    CreatedAt: x.CreateAdt
                ));

                _logger.LogInformation("Retrieved {Count} friendships before the date range: {PastDateRangeType}", 
                                        friendshipDTos.Count(), request.PastDateRangeType);

                return ResultT<IEnumerable<FriendshipDTos>>.Success(friendshipDTos);

            }

            _logger.LogError("Failed to retrieve friendships: Invalid date range type {PastDateRangeType}", request.PastDateRangeType);

            return ResultT<IEnumerable<FriendshipDTos>>.Failure(Error.Failure("400", "Invalid status"));
        }


        private Dictionary<PastDateRangeType, Func<CancellationToken, Task<IEnumerable<Domain.Models.Friendship>>>> GetCreatedBefore()
        {
            return new Dictionary<PastDateRangeType, Func<CancellationToken, Task<IEnumerable<Domain.Models.Friendship>>>>
            {
                {PastDateRangeType.BeforeToday, async cancellationToken => await _friendshipRepository.GetCreatedBeforeAsync(DateTime.UtcNow.Date, cancellationToken) },
                {PastDateRangeType.BeforeWeek, async cancellationToken => await _friendshipRepository.GetCreatedBeforeAsync(DateTime.UtcNow.AddDays(-7), cancellationToken) },
                {PastDateRangeType.BeforeMonth, async cancellationToken => await _friendshipRepository.GetCreatedBeforeAsync(DateTime.UtcNow.AddMonths(-1), cancellationToken) },
                {PastDateRangeType.BeforeYear, async cancellationToken => await _friendshipRepository.GetCreatedBeforeAsync(DateTime.UtcNow.AddYears(-1), cancellationToken) }
            };
        }

    }
}
