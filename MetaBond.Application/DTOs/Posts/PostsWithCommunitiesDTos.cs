
using MetaBond.Application.DTOs.Events;

namespace MetaBond.Application.DTOs.Posts
{
    public sealed record PostsWithCommunitiesDTos
    (
        Guid PostsId,
        string? Title,
        string? Content,
        string? ImageUrl,
        List<CommunitySummaryDto> Communities,
        DateTime? CreatedAt
    );
}
