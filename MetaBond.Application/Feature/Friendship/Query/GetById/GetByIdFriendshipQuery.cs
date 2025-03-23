using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;

namespace MetaBond.Application.Feature.Friendship.Query.GetById;
public sealed class GetByIdFriendshipQuery : IQuery<FriendshipDTos>
{
    public Guid Id { get; set; }
}