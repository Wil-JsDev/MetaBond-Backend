using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.Auth;
using MetaBond.Application.Interfaces.Service.Auth;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Authentication.Commands.RefreshTokenUser;

internal sealed class RefreshTokenUserCommandHandler(
    ILogger<RefreshTokenUserCommandHandler> logger,
    IJwtService jwtService
) : ICommandHandler<RefreshTokenUserCommand, AuthenticationResponse>
{
    public async Task<ResultT<AuthenticationResponse>> Handle(RefreshTokenUserCommand request,
        CancellationToken cancellationToken)
    {
        var newRefreshTokenResult = await jwtService.RefreshUserTokens(request.RefreshToken ?? string.Empty);

        if (newRefreshTokenResult.IsSuccess)
        {
            logger.LogInformation("RefreshTokenUserCommandHandler: User refresh token successful.");

            return ResultT<AuthenticationResponse>.Success(newRefreshTokenResult.Value);
        }

        logger.LogWarning("RefreshTokenUserCommandHandler: Invalid refresh token attempt. Reason: {Reason}",
            newRefreshTokenResult.Error?.Description ?? "Unknown error");

        return ResultT<AuthenticationResponse>.Failure(newRefreshTokenResult.Error ??
                                                       Error.Failure("401", "Invalid refresh token."));
    }
}