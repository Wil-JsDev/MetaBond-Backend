namespace MetaBond.Application.DTOs.ProgressBoard;

public record ProgressEntrySummaryDTos
(
    Guid? ProgressEntryId,
    Guid UserId,
    string? Description,
    DateTime? CreatedAt,
    DateTime? ModifiedAt
);