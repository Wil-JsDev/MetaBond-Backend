using MetaBond.Domain;

namespace MetaBond.Application.DTOs.Account.Chat;

public sealed record LeaveChatDTos(
    Guid ChatId,
    Guid UserId,
    ChatType Type
);