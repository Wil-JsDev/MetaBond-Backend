using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using MetaBond.Domain;

namespace MetaBond.Application.Feature.Friendship.Query.GetCreated.GetCreatedBefore
{
    internal sealed class GetCreatedBeforeFriendshipQueryHandler : IQueryHandler<GetCreatedBeforeFriendshipQuery, IEnumerable<FriendshipDTos>>
    {

        private readonly IFriendshipRepository _friendshipRepository;

        public GetCreatedBeforeFriendshipQueryHandler(IFriendshipRepository friendshipRepository)
        {
            _friendshipRepository = friendshipRepository;
        }

        public async Task<ResultT<IEnumerable<FriendshipDTos>>> Handle(GetCreatedBeforeFriendshipQuery request, CancellationToken cancellationToken)
        {

            var friendshipBefore = GetCreatedBefore();

            if (friendshipBefore.TryGetValue((request.PastDateRangeType), out var beforeAsync))
            {
                var friendshipList = await beforeAsync(cancellationToken);
                if (friendshipList == null || !friendshipList.Any())
                {
                    return ResultT<IEnumerable<FriendshipDTos>>.Failure(Error.Failure("400", "The list is empty"));
                }

                var friendshipDTos = friendshipList.Select(x => new FriendshipDTos
                (
                    FriendshipId: x.Id,
                    Status: x.Status,
                    CreatedAt: x.CreateAt
                ));

                return ResultT<IEnumerable<FriendshipDTos>>.Success(friendshipDTos);

            }

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
