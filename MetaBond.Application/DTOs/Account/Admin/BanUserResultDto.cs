namespace MetaBond.Application.DTOs.Account.Admin;

public sealed record BanUserResultDto(
    Guid UserId,
    string? Username,
    string? StatusUser,
    DateTime BannedAt
);