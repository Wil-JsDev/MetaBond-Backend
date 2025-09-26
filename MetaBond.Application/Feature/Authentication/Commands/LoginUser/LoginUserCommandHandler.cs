using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.Auth;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Interfaces.Service.Auth;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Authentication.Commands.LoginUser;

/// <summary>
/// Handles the login command for users, performing authentication and JWT generation.
/// </summary>
internal sealed class LoginUserCommandHandler(
    ILogger<LoginUserCommandHandler> logger,
    IJwtService jwtService,
    IUserRepository userRepository
) : ICommandHandler<LoginUserCommand, AuthenticationResponse>
{
    public async Task<ResultT<AuthenticationResponse>> Handle(LoginUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email ?? string.Empty, cancellationToken);
        
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user?.Password))
        {
            logger.LogWarning("LoginUserCommandHandler: Invalid credentials attempt.");

            return ResultT<AuthenticationResponse>.Failure(
                Error.Failure("401", "Invalid credentials"));
        }

        if (user.StatusUser == StatusAccount.Banned.ToString())
        {
            logger.LogWarning("LoginUserCommandHandler: Banned user login attempt. UserId: {UserId}", user.Id);
            return ResultT<AuthenticationResponse>.Failure(
                Error.Failure("403", "User is banned"));
        }

        if (!user.IsEmailConfirmed)
        {
            logger.LogWarning("LoginUserCommandHandler: Email not confirmed. UserId: {UserId}", user.Id);
            return ResultT<AuthenticationResponse>.Failure(
                Error.Conflict("409", "Email is not confirmed"));
        }

        var accessToken = jwtService.GenerateTokenUser(user);
        var refreshToken = jwtService.GenerateRefreshTokenUser(user);

        logger.LogInformation("LoginUserCommandHandler: User login successful. UserId: {UserId}", user.Id);

        AuthenticationResponse response = new
        (
            AccessToken: accessToken,
            RefreshToken: refreshToken
        );

        return ResultT<AuthenticationResponse>.Success(response);
    }
}