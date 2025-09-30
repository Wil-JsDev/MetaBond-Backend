namespace MetaBond.Application.DTOs.Account.Auth;

public record JwtResponse(
    bool Success,
    string Error
);