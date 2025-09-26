using System.Security.Claims;
using MetaBond.Application.DTOs.Account.Auth;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Interfaces.Service.Auth;

/// <summary>
/// Provides methods for generating JWT and refresh tokens for users, admins, and community memberships.
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Generates a JWT access token for the specified user.
    /// </summary>
    /// <param name="user">The user for whom to generate the token.</param>
    /// <returns>A JWT access token as a string.</returns>
    string GenerateTokenUser(User user);

    /// <summary>
    /// Generates a refresh token for the specified user.
    /// </summary>
    /// <param name="user">The user for whom to generate the refresh token.</param>
    /// <returns>A refresh token as a string.</returns>
    string GenerateRefreshTokenUser(User user);

    /// <summary>
    /// Generates a JWT access token for the specified admin.
    /// </summary>
    /// <param name="admin">The admin for whom to generate the token.</param>
    /// <returns>A JWT access token as a string.</returns>
    string GenerateTokenAdmin(Admin admin);

    /// <summary>
    /// Generates a refresh token for the specified admin.
    /// </summary>
    /// <param name="admin">The admin for whom to generate the refresh token.</param>
    /// <returns>A refresh token as a string.</returns>
    string GenerateRefreshTokenAdmin(Admin admin);

    /// <summary>
    /// Generates a JWT access token for a user within a specific community membership.
    /// </summary>
    /// <param name="user">The user for whom to generate the token.</param>
    /// <param name="community">The community membership context.</param>
    /// <param name="role"></param>
    /// <returns>A JWT access token as a string.</returns>
    string GenerateTokenCommunity(User user, CommunityMembership community, string role);

    /// <summary>
    /// Refreshes the access and refresh tokens for an admin using a valid refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token string.</param>
    /// <returns>A ResultT containing the new AuthenticationResponse or a failure result.</returns>
    ResultT<AuthenticationResponse> RefreshAdminTokens(string refreshToken);

    /// <summary>
    /// Refreshes the access and refresh tokens for a user using a valid refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token string.</param>
    /// <returns>A ResultT containing the new AuthenticationResponse or a failure result.</returns>
    ResultT<AuthenticationResponse> RefreshUserTokens(string refreshToken);

    /// <summary>
    /// Validates and extracts the principal from an expired JWT token. Returns a failure result if the token is invalid or malformed.
    /// </summary>
    /// <param name="token">The JWT token string.</param>
    /// <returns>A ResultT containing the ClaimsPrincipal if valid, or a failure result.</returns>
    ResultT<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token);
}