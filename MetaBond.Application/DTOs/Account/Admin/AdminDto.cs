namespace MetaBond.Application.DTOs.Account.Admin;

public sealed record AdminDto(
    Guid AdminId,
    string? FirstName,
    string? LastName,
    string? Username,
    string? Photo
);