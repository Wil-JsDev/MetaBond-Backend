namespace MetaBond.Application.DTOs.Account.User;

public sealed record UserProgressEntryDTos(
    Guid UserId,
    string? Username,
    string? Photo
);