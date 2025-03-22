
namespace MetaBond.Application.DTOs.ProgressBoard
{
    public sealed record ProgressBoardWithProgressEntryDTos
    (
        Guid ProgressBoardId,
        Guid CommunitiesId,
        List<ProgressEntrySummaryDTos> ProgressEntries,
        DateTime? CreatedAt,
        DateTime? UpdatedAt
    );
}
