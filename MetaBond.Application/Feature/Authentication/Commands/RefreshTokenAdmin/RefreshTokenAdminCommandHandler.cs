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
    public Task<ResultT<AuthenticationResponse>> Handle(RefreshTokenAdminCommand request,
        CancellationToken cancellationToken)
    {
        var newRefreshTokenResult = jwtService.RefreshAdminTokens(request.RefreshToken ?? string.Empty);

        if (newRefreshTokenResult.IsSuccess) return Task.FromResult(newRefreshTokenResult);

        logger.LogWarning("RefreshTokenAdminCommandHandler: Invalid refresh token attempt. Reason: {Reason}",
            newRefreshTokenResult.Error?.Description ?? "Unknown error");

        return Task.FromResult(ResultT<AuthenticationResponse>.Failure(
            newRefreshTokenResult.Error ?? Error.Failure("401", "Invalid refresh token.")));
    }
}