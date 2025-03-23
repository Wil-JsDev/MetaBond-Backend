using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Domain;

namespace MetaBond.Application.Feature.Friendship.Query.GetCreated.GetCreatedAfter;

public sealed class GetCreatedAfterFriendshipQuery : IQuery<IEnumerable<FriendshipDTos>>
{
    public DateRangeType DateRange { get; set; }
}