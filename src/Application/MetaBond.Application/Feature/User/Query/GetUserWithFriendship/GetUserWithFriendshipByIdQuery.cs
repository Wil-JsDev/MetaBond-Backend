using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.User;

namespace MetaBond.Application.Feature.User.Query.GetUserWithFriendship;

public sealed class GetUserWithFriendshipByIdQuery : IQuery<UserWithFriendshipDTos>
{
    public Guid? UserId { get; set; }
}