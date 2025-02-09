
namespace MetaBond.Application.DTOs.ProgressEntry
{
    public sealed record ProgressEntryDTos
    (
        Guid ProgressEntryId,
        Guid ProgressBoardId,
        string? Description,
        DateTime CreatedAt,
        DateTime UpdateAt
    );
}
