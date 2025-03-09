
namespace MetaBond.Application.DTOs.Posts
{
    public sealed record PostsWithCommunitiesDTos
    (
        Guid PostsId,
        string? Title,
        string? Content,
        string? ImageUrl,
        Domain.Models.Communities? Communities,
        DateTime? CreatedAt
    );
}
