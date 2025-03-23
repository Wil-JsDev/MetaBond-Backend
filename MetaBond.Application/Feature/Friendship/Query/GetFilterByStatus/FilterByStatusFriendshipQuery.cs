using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Domain;

namespace MetaBond.Application.Feature.Friendship.Query.GetFilterByStatus;

public sealed class FilterByStatusFriendshipQuery : IQuery<IEnumerable<FriendshipDTos>>
{
    public Status Status { get; set; }
}