namespace MetaBond.Application.DTOs.Account.User;

public record UserProgressEntryDTos
(
    Guid UserId,
    string? Username,
    string? Photo
);