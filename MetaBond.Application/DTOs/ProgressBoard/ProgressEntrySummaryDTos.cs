namespace MetaBond.Application.DTOs.ProgressBoard;

public record ProgressEntrySummaryDTos
(
    Guid? ProgressEntryId,
    string? Description,
    DateTime? CreatedAt,
    DateTime? ModifiedAt
);