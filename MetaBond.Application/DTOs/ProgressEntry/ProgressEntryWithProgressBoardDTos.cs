using MetaBond.Application.DTOs.ProgressBoard;

namespace MetaBond.Application.DTOs.ProgressEntry;

public sealed record ProgressEntryWithProgressBoardDTos
(
    Guid ProgressEntryId,
    Guid UserId,
    ProgressBoardSummaryDTos ProgressBoard,
    string? Description,
    DateTime CreatedAt,
    DateTime UpdateAt
);