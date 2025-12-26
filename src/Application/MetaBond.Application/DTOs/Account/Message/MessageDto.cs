namespace MetaBond.Application.DTOs.Account.Message;

public sealed record MessageDto(
    Guid MessageId,
    Guid UserId,
    string Content,
    DateTime? SentAt);