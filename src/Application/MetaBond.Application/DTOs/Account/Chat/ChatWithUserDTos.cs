using MetaBond.Domain;

namespace MetaBond.Application.DTOs.Account.Chat;

public record ChatWithUserDTos(
    Guid ChatId,
    ChatType Type,
    string? NameGroup,
    string? PhotoGroup,
    UserChatDTos? User
);

public record UserChatDTos(
    Guid UserId,
    string? Username,
    string? Photo,
    string? FullName
);