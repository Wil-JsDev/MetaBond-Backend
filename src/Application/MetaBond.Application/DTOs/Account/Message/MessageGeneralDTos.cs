using MetaBond.Application.DTOs.Account.User;

namespace MetaBond.Application.DTOs.Account.Message;

public sealed record MessageGeneralDTos(
    Guid MessageId,
    Guid ChatId,
    string Content,
    DateTime? SentAt,
    bool IsEdited,
    DateTime? EditedAt,
    bool IsDeleted,
    SenderDTos Sender
);