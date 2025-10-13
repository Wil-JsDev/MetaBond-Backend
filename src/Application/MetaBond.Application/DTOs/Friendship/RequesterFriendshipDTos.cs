using MetaBond.Domain;

namespace MetaBond.Application.DTOs.Friendship;

public record RequesterFriendshipDTos(
    Guid FriendshipId,
    Guid RequesterId,
    string Username,
    Status StatusFriendship
);