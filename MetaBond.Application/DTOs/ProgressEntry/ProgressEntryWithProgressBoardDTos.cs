using MetaBond.Application.DTOs.ProgressBoard;

namespace MetaBond.Application.DTOs.ProgressEntry;

public record ProgressEntryWithProgressBoardDTos
(
    Guid ProgressEntryId,
    ProgressBoardSummaryDTos ProgressBoard,
    string? Description,
    DateTime CreatedAt,
    DateTime UpdateAt
);