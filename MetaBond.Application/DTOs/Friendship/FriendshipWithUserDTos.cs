using MetaBond.Application.DTOs.Account.User;
using MetaBond.Domain;

namespace MetaBond.Application.DTOs.Friendship;

public record FriendshipWithUserDTos(
    Guid FriendshipId,
    Status Status,
    Guid? RequesterId,
    Guid? AddresseeId,
    UserFriendshipDTos? User,
    DateTime? CreatedAt
);