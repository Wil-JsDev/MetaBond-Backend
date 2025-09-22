namespace MetaBond.Application.DTOs.Account.User;

public sealed record UserDTos(
    Guid UserId,
    string? FirstName,
    string? LastName,
    string? Username,
    string? Photo,
    string? StatusAccount,
    DateTime? CreatedAt,
    DateTime? UpdateAt
);