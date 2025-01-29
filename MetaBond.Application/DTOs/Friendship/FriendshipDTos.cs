using MetaBond.Domain;

namespace MetaBond.Application.DTOs.Friendship
{
    public sealed record FriendshipDTos
    (
        Guid FriendshipId,
        Status Status,
        DateTime? CreatedAt
    );
}
