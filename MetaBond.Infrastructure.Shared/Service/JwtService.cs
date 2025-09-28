using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MetaBond.Application.DTOs.Account.Auth;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Interfaces.Service.Auth;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using MetaBond.Domain.Models;
using MetaBond.Domain.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MetaBond.Infrastructure.Shared.Service;

/// <summary>
/// Service for generating JWT and refresh tokens for users, admins, and community memberships.
/// </summary>
public class JwtService(
    IOptions<JwtSettings> jwtSettings,
    IAdminRepository adminRepository,
    IUserRepository userRepository) : IJwtService
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;
    private const string AccessType = "access";
    private const string RefreshType = "refresh";
    private const string CommunityType = "community";

    public string GenerateTokenUser(User user)
    {
        ValidateSettings();
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Role, UserRoles.User.ToString()),
            new Claim("type", AccessType)
        };
        return BuildToken(claims, TimeSpan.FromMinutes(_jwtSettings.ExpirationInMinutes));
    }

    public string GenerateRefreshTokenUser(User user)
    {
        ValidateSettings();
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("type", RefreshType)
        };
        return BuildToken(claims, TimeSpan.FromDays(_jwtSettings.RefreshTokenExpirationInDays));
    }

    public string GenerateTokenAdmin(Admin admin)
    {
        ValidateSettings();
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, admin.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Email, admin.Email ?? string.Empty),
            new Claim(ClaimTypes.Role, UserRoles.Admin.ToString()),
            new Claim("type", AccessType)
        };
        return BuildToken(claims, TimeSpan.FromMinutes(_jwtSettings.ExpirationInMinutes));
    }

    public string GenerateRefreshTokenAdmin(Admin admin)
    {
        ValidateSettings();
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, admin.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("type", RefreshType)
        };
        return BuildToken(claims, TimeSpan.FromDays(_jwtSettings.RefreshTokenExpirationInDays));
    }

    public string GenerateTokenCommunity(User user, CommunityMembership communityMembership, Communities community,
        string role)
    {
        ValidateSettings();
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, communityMembership.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, role),
            new Claim("userId", user.Id.ToString()),
            new Claim("communityId", community.Id.ToString()),
            new Claim("type", CommunityType)
        };
        return BuildToken(claims, TimeSpan.FromMinutes(_jwtSettings.ExpirationInMinutes));
    }

    public ResultT<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return ResultT<ClaimsPrincipal>.Failure(Error.Failure("400", "Token is required."));
        }

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(_jwtSettings.Key ?? string.Empty)),
            ValidateLifetime = false // Allow expired tokens
        };

        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        var tokenHandler = new JwtSecurityTokenHandler();

        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
        {
            return ResultT<ClaimsPrincipal>.Failure(Error.Failure("401", "Invalid token algorithm."));
        }

        return ResultT<ClaimsPrincipal>.Success(principal);
    }

    public async Task<ResultT<AuthenticationResponse>> RefreshUserTokens(string refreshToken)
    {
        return await RefreshTokensInternal(refreshToken, isAdmin: false);
    }

    public async Task<ResultT<AuthenticationResponse>> RefreshAdminTokens(string refreshToken)
    {
        return await RefreshTokensInternal(refreshToken, isAdmin: true);
    }

    #region Private Methods

    /// <summary>
    /// Internal logic for refreshing tokens for user or admin.
    /// </summary>
    private async Task<ResultT<AuthenticationResponse>> RefreshTokensInternal(string refreshToken, bool isAdmin)
    {
        var principalResult = GetPrincipalFromExpiredToken(refreshToken);
        if (principalResult is { IsSuccess: false })
        {
            return ResultT<AuthenticationResponse>.Failure(
                Error.Failure("401", "Refresh token is invalid or malformed."));
        }

        var principal = principalResult.Value;
        var typeClaim = principal.Claims.FirstOrDefault(c => c.Type == "type")?.Value;
        if (typeClaim != RefreshType)
        {
            return ResultT<AuthenticationResponse>.Failure(
                Error.Failure("401", "Provided token is not a refresh token."));
        }

        var sub = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrEmpty(sub))
        {
            return ResultT<AuthenticationResponse>.Failure(
                Error.Failure("401", "Refresh token is missing subject identifier."));
        }

        if (!Guid.TryParse(sub, out var id))
        {
            return ResultT<AuthenticationResponse>.Failure(
                Error.Failure("400", "Subject identifier is not a valid GUID."));
        }

        string newToken, newRefreshToken;
        if (isAdmin)
        {
            var admin = await adminRepository.GetByIdAsync(id);
            if (admin is null)
            {
                return ResultT<AuthenticationResponse>.Failure(
                    Error.Failure("400", "Admin not found."));
            }

            newToken = GenerateTokenAdmin(admin);
            newRefreshToken = GenerateRefreshTokenAdmin(admin);
        }
        else
        {
            var user = await userRepository.GetByIdAsync(id);
            if (user is null)
            {
                return ResultT<AuthenticationResponse>.Failure(
                    Error.Failure("400", "User not found."));
            }

            newToken = GenerateTokenUser(user);
            newRefreshToken = GenerateRefreshTokenUser(user);
        }

        AuthenticationResponse response = new(
            AccessToken: newToken,
            RefreshToken: newRefreshToken
        );

        return ResultT<AuthenticationResponse>.Success(response);
    }

    private void ValidateSettings()
    {
        if (string.IsNullOrWhiteSpace(_jwtSettings.Key) ||
            string.IsNullOrWhiteSpace(_jwtSettings.Issuer) ||
            string.IsNullOrWhiteSpace(_jwtSettings.Audience) ||
            _jwtSettings.ExpirationInMinutes <= 0)
        {
            throw new InvalidOperationException("JWT settings are not properly configured.");
        }
    }

    private string BuildToken(IEnumerable<Claim> claims, TimeSpan expiresIn)
    {
        var key = new SymmetricSecurityKey(Convert.FromBase64String(_jwtSettings.Key ?? string.Empty));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(expiresIn),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    #endregion
}