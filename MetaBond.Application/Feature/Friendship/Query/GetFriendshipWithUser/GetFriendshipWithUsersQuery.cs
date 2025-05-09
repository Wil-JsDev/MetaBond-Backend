using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;

namespace MetaBond.Application.Feature.Friendship.Query.GetFriendshipWithUser;

public sealed class GetFriendshipWithUsersQuery : IQuery<IEnumerable<FriendshipWithUserDTos>>
{
    public Guid? FriendshipId { get; set; }
}