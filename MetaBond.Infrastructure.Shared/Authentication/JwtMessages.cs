namespace MetaBond.Infrastructure.Shared.Authentication;

public static class JwtMessages
{
    // Authentication
    public const string AuthenticationFailed = "Authentication failed. Invalid or malformed token.";
    public const string TokenExpired = "Token has expired.";
    public const string InvalidSignature = "Invalid token signature.";
    public const string MissingToken = "Authorization token missing or invalid.";
    public const string InvalidToken = "Token is invalid or has been tampered with.";

    // Authorization
    public const string AccessDenied = "Access denied.";
    public const string Forbidden = "You do not have permission to perform this action.";
    public const string UnauthorizedRole = "Your role does not grant access to this resource.";

    // ⚙️ Validation
    public const string TokenValidated = "Token successfully validated.";
    public const string TokenRevoked = "This token has been revoked.";
    public const string TokenNotYetValid = "Token is not yet valid.";

    // User
    public const string UserInactive = "User is no longer active.";
}