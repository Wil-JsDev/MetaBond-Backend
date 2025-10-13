namespace MetaBond.Application.DTOs.Account.Admin;

public sealed record UnbanUserResultDto(
    Guid UserId,
    string? Username,
    string? StatusUser
);