using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Email;
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
        if (request != null)
        {
            var user = await userRepository.GetByEmailAsync(request!.Email!, cancellationToken);
            if (user == null)
            {
                logger.LogError("Email {Email} not found during forgot password request", request.Email);

                return ResultT<string>.Failure(Error.NotFound("404", $"No user found with email {request.Email}"));
            }

            var code = await emailConfirmationTokenService.GenerateTokenAsync(user.Id, cancellationToken);

            await emailService.SendEmailAsync(new EmailRequestDTo(
                To: user.Email,
                Body: EmailTemplates.GetPasswordRecoveryEmailHtml(user.Username!, code.Value),
                Subject: "Forgot Password"
            ));

            logger.LogInformation("Password reset code generated and email sent to {Email}", user.Email);

            return ResultT<string>.Success("Password reset email sent successfully.");
        }

        logger.LogWarning("Forgot password request is null");

        return ResultT<string>.Failure(Error.Failure("400", "Request cannot be null"));
    }
}