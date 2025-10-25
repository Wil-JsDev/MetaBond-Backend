namespace MetaBond.Application.DTOs.GitHub;

public sealed record GitHubResponseDTos(
    string AccessToken,
    string? RefreshToken,
    Guid UserId
);