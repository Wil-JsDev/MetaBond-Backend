using MetaBond.Application.DTOs.Account.User;

namespace MetaBond.Application.DTOs.ProgressEntry;

public record ProgressEntriesWithUserDTos
(
    Guid ProgressEntryId,
    Guid ProgressBoardId,
    UserProgressEntryDTos User,
    string? Description,
    DateTime CreatedAt,
    DateTime UpdateAt
);