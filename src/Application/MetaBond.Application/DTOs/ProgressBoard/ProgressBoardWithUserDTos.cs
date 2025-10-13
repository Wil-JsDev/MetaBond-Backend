using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.DTOs.Communities;
using MetaBond.Application.DTOs.ProgressEntry;

namespace MetaBond.Application.DTOs.ProgressBoard;

public sealed record ProgressBoardWithUserDTos(
    Guid ProgressBoardId,
    ProgressEntryWithBoardDTos? ProgressEntry,
    CommunitiesDTos Communities,
    UserProgressEntryDTos User,
    DateTime? CreatedAt
);