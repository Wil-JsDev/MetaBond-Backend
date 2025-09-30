using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.User.Commands.ResetPassword;

internal sealed class ResetPasswordUserCommandHandler(
    ILogger<ResetPasswordUserCommandHandler> logger,
    IUserRepository userRepository
) : ICommandHandler<ResetPasswordUserCommand, string>
{
    public async Task<ResultT<string>> Handle(ResetPasswordUserCommand request, CancellationToken cancellationToken)
    {
        var user = await EntityHelper.GetEntityByIdAsync(
            userRepository.GetByIdAsync,
            request.UserId,
            "User",
            logger
        );

        if (!user.IsSuccess) return user.Error!;

        // Password validation should be handled by a validator
        var hashPassword = BCrypt.Net.BCrypt.HashPassword(request.ConfirmNewPassword);

        await userRepository.UpdatePasswordAsync(user.Value, hashPassword, cancellationToken);

        logger.LogInformation("Password has been successfully updated.");

        return ResultT<string>.Success("Password has been successfully updated.");
    }
}