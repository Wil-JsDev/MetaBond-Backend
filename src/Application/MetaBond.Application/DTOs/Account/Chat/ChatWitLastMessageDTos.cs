using MetaBond.Domain;

namespace MetaBond.Application.DTOs.Account.Chat;

public sealed record ChatWitLastMessageDTos(
    Guid ChatId,
    ChatType Type,
    string LastMessage
);