
namespace MetaBond.Application.DTOs.ProgressBoard
{
    public sealed record ProgressBoardWithProgressEntryDTos
    (
        Guid ProgressBoardId,
        Guid CommunitiesId,
        ICollection<Domain.Models.ProgressEntry> ProgressEntries,
        DateTime? CreatedAt,
        DateTime? UpdatedAt
    );
}
