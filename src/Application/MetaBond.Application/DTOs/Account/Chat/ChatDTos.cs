using MetaBond.Domain;

namespace MetaBond.Application.DTOs.Account.Chat;

public sealed record ChatDTos(
    Guid ChatId,
    ChatType Type,
    string? Name,
    string? Photo
);