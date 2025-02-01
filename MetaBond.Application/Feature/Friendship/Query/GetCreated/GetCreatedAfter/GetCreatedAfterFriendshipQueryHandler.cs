using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Friendship.Query.GetCreated.GetCreatedAfter
{
    internal sealed class GetCreatedAfterFriendshipQueryHandler : IQueryHandler<GetCreatedAfterFriendshipQuery, IEnumerable<FriendshipDTos>>
    {
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly ILogger<GetCreatedAfterFriendshipQueryHandler> _logger;

        public GetCreatedAfterFriendshipQueryHandler(
            IFriendshipRepository friendshipRepository, 
            ILogger<GetCreatedAfterFriendshipQueryHandler> logger)
        {
            _friendshipRepository = friendshipRepository;
            _logger = logger;
        }

        public async Task<ResultT<IEnumerable<FriendshipDTos>>> Handle(
            GetCreatedAfterFriendshipQuery request, 
            CancellationToken cancellationToken)
        {
            var friendshipAfter = GetCreatedAfterFriendship();
            DateTime dateTime = DateTime.UtcNow;
            if (friendshipAfter.TryGetValue((request.DateRange), out var createdAfter))
            {
                var friendshipList = await createdAfter(cancellationToken);
                if (friendshipList == null || !friendshipList.Any())
                {
                    _logger.LogError("No friendships found after the specified date range: {DateRange}", request.DateRange);

                    return ResultT<IEnumerable<FriendshipDTos>>.Failure(Error.Failure("400", "The list is empty"));
                }

                IEnumerable<FriendshipDTos> friendshipDTos = friendshipList.Select(x => new FriendshipDTos
                (
                    FriendshipId: x.Id,
                    Status: x.Status,
                    CreatedAt: x.CreateAdt
                ));

                _logger.LogInformation("Retrieved {Count} friendships after the date range: {DateRange}", 
                                        friendshipDTos.Count(), request.DateRange);

                return ResultT<IEnumerable<FriendshipDTos>>.Success(friendshipDTos);
            }

            _logger.LogError("Failed to retrieve friendships: Invalid date range type {DateRange}", request.DateRange);

            return ResultT<IEnumerable<FriendshipDTos>>.Failure(Error.Failure("400", "Invalid status"));
        }

        private Dictionary<DateRangeType, Func<CancellationToken, Task<IEnumerable<Domain.Models.Friendship>>>> GetCreatedAfterFriendship()
        {
            return new Dictionary<DateRangeType, Func<CancellationToken, Task<IEnumerable<Domain.Models.Friendship>>>>()
            {
            { DateRangeType.Today, async cancellationToken => await _friendshipRepository.GetCreatedAfterAsync(DateTime.UtcNow.Date, cancellationToken) },
            { DateRangeType.Week, async cancellationToken => await _friendshipRepository.GetCreatedAfterAsync(DateTime.UtcNow.AddDays(-7), cancellationToken) },
            { DateRangeType.Month, async cancellationToken => await _friendshipRepository.GetCreatedAfterAsync(DateTime.UtcNow.AddMonths(-1), cancellationToken) },
            { DateRangeType.Year, async cancellationToken => await _friendshipRepository.GetCreatedAfterAsync(DateTime.UtcNow.AddYears(-1), cancellationToken) }
            };
        }

    }
}
