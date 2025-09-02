using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Interfaces.Service;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.User.Commands.ConfirmAccount;

internal sealed class ConfirmAccountCommandHandler(
    IUserRepository userRepository,
    IEmailConfirmationTokenService emailConfirmationTokenService,
    ILogger<ConfirmAccountCommandHandler> logger
) : ICommandHandler<ConfirmAccountCommand, string>
{
    public async Task<ResultT<string>> Handle(
        ConfirmAccountCommand request,
        CancellationToken cancellationToken
    )
    {
        if (request == null)
        {
            logger.LogError("ConfirmAccountCommand: Request payload is null.");

            return ResultT<string>.Failure(Error.Failure("400", "The request cannot be null."));
        }

        var userId = request.UserId ?? Guid.Empty;

        var user = await EntityHelper.GetEntityByIdAsync(
            userRepository.GetByIdAsync,
            userId,
            "User",
            logger
        );

        if (!user.IsSuccess)
            return user.Error!;

        var result = await emailConfirmationTokenService.ConfirmAccountAsync(userId, request.Code!, cancellationToken);

        if (!result.IsSuccess)
        {
            logger.LogError(
                "ConfirmAccountCommand: Failed to confirm account for user '{UserId}' using code '{Code}'. Reason: {ErrorMessage}",
                userId, request.Code, result.Error);

            return ResultT<string>.Failure(result.Error!);
        }

        logger.LogInformation(
            "ConfirmAccountCommand: Account confirmation successful for user '{UserId}' with code '{Code}'.",
            userId, request.Code);

        return ResultT<string>.Success("Account confirmed successfully.");
    }
}