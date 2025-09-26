namespace MetaBond.Application.DTOs.Account.Auth;

public sealed record AuthenticationRequest(
    string Email,
    string Password
);