using MetaBond.Domain;

namespace MetaBond.Application.DTOs.Friendship
{
    public sealed record FriendshipDTos
    (
        Guid FriendshipId,
        Status Status,
        Guid? RequesterId,
        Guid? AddresseeId,
        DateTime? CreatedAt
    );
}
