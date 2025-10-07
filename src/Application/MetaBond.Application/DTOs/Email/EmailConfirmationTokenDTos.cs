namespace MetaBond.Application.DTOs.Email;

public sealed record EmailConfirmationTokenDTos(
    Guid EmailConfirmationTokenId,
    Guid UserId,
    string Token,
    bool IsUsed,
    DateTime ExpiresAt
);