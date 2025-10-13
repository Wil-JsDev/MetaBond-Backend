namespace MetaBond.Application.DTOs.ProgressEntry;

public sealed record ProgressEntryDTos(
    Guid ProgressEntryId,
    Guid ProgressBoardId,
    Guid UserId,
    string? Description,
    DateTime CreatedAt,
    DateTime UpdateAt
);