using MetaBond.Application.DTOs.Account.User;

namespace MetaBond.Application.DTOs.Posts
{
    public sealed record PostsDTos(
        Guid PostsId,
        string? Title,
        string? Content,
        string? ImageUrl,
        Guid? CreatedById,
        Guid? CommunitiesId,
        DateTime? CreatedAt
    );
}