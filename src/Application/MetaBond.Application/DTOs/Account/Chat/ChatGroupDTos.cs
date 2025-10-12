using MetaBond.Domain;

namespace MetaBond.Application.DTOs.Account.Chat;

public sealed record ChatGroupDTos(
    Guid ChatId,
    ChatType Type,
    string? Name,
    string? Photo
);