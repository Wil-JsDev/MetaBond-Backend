namespace MetaBond.Application.DTOs.ProgressBoard;

public sealed record ProgressEntrySummaryDTos
(
    Guid? ProgressEntryId,
    Guid UserId,
    string? Description,
    DateTime? CreatedAt,
    DateTime? ModifiedAt
);