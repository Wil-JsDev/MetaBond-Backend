using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;

namespace MetaBond.Application.Feature.Friendship.Query.GetRecentlyCreated;

public sealed class GetRecentlyCreatedFriendshipQuery : IQuery<IEnumerable<FriendshipDTos>>
{
    public int Limit { get; set; }
}