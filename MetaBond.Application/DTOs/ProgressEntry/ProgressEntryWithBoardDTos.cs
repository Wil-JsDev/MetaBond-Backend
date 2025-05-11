using MetaBond.Application.DTOs.ProgressBoard;

namespace MetaBond.Application.DTOs.ProgressEntry;

public record ProgressEntryWithBoardDTos
(
    Guid ProgressEntryId, 
    string? Description,
    Guid UserId
);