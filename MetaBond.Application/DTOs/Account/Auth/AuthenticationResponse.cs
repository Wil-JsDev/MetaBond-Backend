namespace MetaBond.Application.DTOs.Account.Auth;

public sealed record AuthenticationResponse(
    string AccessToken,
    string? RefreshToken);