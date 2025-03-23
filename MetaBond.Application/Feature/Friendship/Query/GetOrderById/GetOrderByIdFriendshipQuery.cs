using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;

namespace MetaBond.Application.Feature.Friendship.Query.GetOrderById;

public sealed class GetOrderByIdFriendshipQuery : IQuery<IEnumerable<FriendshipDTos>>
{
    public string? Sort {  get; set; }
}