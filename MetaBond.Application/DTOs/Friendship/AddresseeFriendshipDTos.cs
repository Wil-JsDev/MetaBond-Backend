using MetaBond.Domain;

namespace MetaBond.Application.DTOs.Friendship;

public record AddresseeFriendshipDTos(
    Guid FriendshipId,
    Guid AddresseeId,
    string? Username,
    Status StatusFriendship
);