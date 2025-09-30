using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.Auth;
using MetaBond.Application.Interfaces.Service.Auth;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Authentication.Commands.RefreshTokenAdmin;

internal sealed class
    RefreshTokenAdminCommandHandler(
        ILogger<RefreshTokenAdminCommandHandler> logger,
        IJwtService jwtService
    ) : ICommandHandler<RefreshTokenAdminCommand, AuthenticationResponse>
{
    public async Task<ResultT<AuthenticationResponse>> Handle(RefreshTokenAdminCommand request,
        CancellationToken cancellationToken)
    {
        var newRefreshTokenResult = await jwtService.RefreshAdminTokens(request.RefreshToken ?? string.Empty);

        if (newRefreshTokenResult.IsSuccess)
        {
            logger.LogInformation("RefreshTokenAdminCommandHandler: Admin refresh token successful.");

            return ResultT<AuthenticationResponse>.Success(newRefreshTokenResult.Value);
        }

        logger.LogWarning("RefreshTokenAdminCommandHandler: Invalid refresh token attempt. Reason: {Reason}",
            newRefreshTokenResult.Error?.Description ?? "Unknown error");

        return ResultT<AuthenticationResponse>.Failure(
            newRefreshTokenResult.Error ?? Error.Failure("401", "Invalid refresh token."));
    }
}