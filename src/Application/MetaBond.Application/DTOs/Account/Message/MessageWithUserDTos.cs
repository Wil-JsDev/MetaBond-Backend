using MetaBond.Application.DTOs.Account.User;

namespace MetaBond.Application.DTOs.Account.Message;

public sealed record MessageWithUserDTos(
    Guid MessageId,
    string Content,
    DateTime? SentAt,
    SenderDTos Sender
);