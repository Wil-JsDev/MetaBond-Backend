using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Domain;

namespace MetaBond.Application.Feature.Friendship.Query.GetCountByStatus
{
    public sealed class GetCountByStatusFriendshipQuery : IQuery<int>
    {
        public Status Status { get; set; }
    }
}
