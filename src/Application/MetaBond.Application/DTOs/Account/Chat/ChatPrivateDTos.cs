using MetaBond.Application.DTOs.Account.Message;
using MetaBond.Domain;

namespace MetaBond.Application.DTOs.Account.Chat;

public sealed record ChatPrivateDTos(
    Guid ChatId,
    ChatType Type
);