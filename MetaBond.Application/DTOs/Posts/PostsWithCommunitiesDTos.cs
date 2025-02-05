
namespace MetaBond.Application.DTOs.Posts
{
    public sealed record PostsWithCommunitiesDTos
    (
        Guid PostsId,
        string? Title,
        string? Content,
        string? ImageUrl,
        Guid? CommunitiesId,
        Domain.Models.Communities? Communities,
        DateTime? CreatedAt
    );
}
