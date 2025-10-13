using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Email;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Interfaces.Service;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.User.Commands.ForgotPassword;

internal sealed class ForgotPasswordUserCommandHandler(
    IUserRepository userRepository,
    ILogger<ForgotPasswordUserCommandHandler> logger,
    IEmailService emailService,
    IEmailConfirmationTokenService emailConfirmationTokenService
) :
    ICommandHandler<ForgotPasswordUserCommand, string>
{
    public async Task<ResultT<string>> Handle(
        ForgotPasswordUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await EntityHelper.GetEntityByIdAsync(
            userRepository.GetByIdAsync,
            request.UserId ?? Guid.Empty,
            "User",
            logger
        );

        if (!user.IsSuccess)
            return user.Error!;

        var code = await emailConfirmationTokenService.GenerateTokenAsync(request.UserId ?? Guid.Empty,
            cancellationToken);

        await emailService.SendEmailAsync(new EmailRequestDTo(
            To: user.Value.Email,
            Body: EmailTemplates.GetPasswordRecoveryEmailHtml(user.Value.Username!, code.Value),
            Subject: "Forgot Password"
        ));

        logger.LogInformation("Password reset code generated and email sent to {Email}", user.Value.Email);

        return ResultT<string>.Success("Password reset email sent successfully.");
    }
}