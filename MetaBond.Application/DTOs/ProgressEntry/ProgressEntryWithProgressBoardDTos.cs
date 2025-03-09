namespace MetaBond.Application.DTOs.ProgressEntry;

public record ProgressEntryWithProgressBoardDTos
(
    Guid ProgressEntryId,
    Domain.Models.ProgressBoard ProgressBoard,
    string Description,
    DateTime CreatedAt,
    DateTime UpdateAt
);