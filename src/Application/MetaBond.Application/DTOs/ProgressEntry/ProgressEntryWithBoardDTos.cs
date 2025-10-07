using MetaBond.Application.DTOs.ProgressBoard;

namespace MetaBond.Application.DTOs.ProgressEntry;

public sealed record ProgressEntryWithBoardDTos(
    Guid ProgressEntryId,
    string? Description,
    Guid UserId
);