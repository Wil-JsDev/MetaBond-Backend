namespace MetaBond.Application.DTOs.Account.User;

public sealed record UserPostsDTos(
    Guid? UserId,
    string? FirstName,
    string? Username,
    string? LastName,
    string? Photo
);