namespace MetaBond.Application.DTOs.Account.Password;

public sealed record UpdatePasswordParameter(
    string? NewPassword,
    string? NewPasswordConfirmPassword
);