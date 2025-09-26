using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.Auth;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Interfaces.Service.Auth;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Authentication.Commands.LoginAdmin;

/// <summary>
/// Handles the login command for admins, performing authentication and JWT generation.
/// </summary>
internal sealed class LoginAdminCommandHandler(
    ILogger<LoginAdminCommandHandler> logger,
    IAdminRepository adminRepository,
    IJwtService jwtService
) : ICommandHandler<LoginAdminCommand, AuthenticationResponse>
{
    public async Task<ResultT<AuthenticationResponse>> Handle(LoginAdminCommand request,
        CancellationToken cancellationToken)
    {
        // Consider implementing rate limiting or lockout after several failed attempts for security.
        var admin = await adminRepository.GetByEmailAsync(request.Email ?? string.Empty, cancellationToken);
        if (admin is null || !BCrypt.Net.BCrypt.Verify(request.Password, admin.Password))
        {
            logger.LogWarning("LoginAdminCommandHandler: Invalid admin credentials attempt.");
            return ResultT<AuthenticationResponse>.Failure(
                Error.Failure("401", "Invalid credentials"));
        }

        var accessToken = jwtService.GenerateTokenAdmin(admin);
        var refreshToken = jwtService.GenerateRefreshTokenAdmin(admin);

        logger.LogInformation("LoginAdminCommandHandler: Admin login successful. AdminId: {AdminId}", admin.Id);

        AuthenticationResponse response = new
        (
            AccessToken: accessToken,
            RefreshToken: refreshToken
        );

        return ResultT<AuthenticationResponse>.Success(response);
    }
}