using MetaBond.Application.DTOs.Friendship;

namespace MetaBond.Application.DTOs.Account.User;

public sealed record UserWithFriendshipDTos(
    Guid UserId,
    string? FirstName,
    string? LastName,
    string? Photo,
    List<RequesterFriendshipDTos> Requester,
    List<AddresseeFriendshipDTos> Addressee
);