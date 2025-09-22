using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Interfaces.Service;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Admin.Commands.ConfirmAccount;

internal sealed class ConfirmAccountAdminCommandHandler(
    IAdminRepository adminRepository,
    ILogger<ConfirmAccountAdminCommandHandler> logger,
    IEmailConfirmationTokenService emailConfirmationTokenService
) : ICommandHandler<ConfirmAccountAdminCommand, string>
{
    public async Task<ResultT<string>> Handle(ConfirmAccountAdminCommand request, CancellationToken cancellationToken)
    {
        var admin = await EntityHelper.GetEntityByIdAsync(
            adminRepository.GetByIdAsync,
            request.AdminId ?? Guid.Empty,
            "Admin",
            logger);

        if (!admin.IsSuccess) return admin.Error!;

        var resultToken =
            await emailConfirmationTokenService.ConfirmAccountAsync(request.AdminId ?? Guid.Empty, request.Code,
                cancellationToken);

        if (!resultToken.IsSuccess)
        {
            logger.LogError(
                "ConfirmAccountAdminCommand: Failed to confirm account for admin '{AdminId}' using code '{Code}'. Reason: {ErrorMessage}",
                request.AdminId, request.Code, resultToken.Error);

            return resultToken.Error!;
        }

        logger.LogInformation("ConfirmAccountAdminCommand: Account for admin '{AdminId}' confirmed successfully.",
            request.AdminId);

        return ResultT<string>.Success("Account confirmed successfully.");
    }
}