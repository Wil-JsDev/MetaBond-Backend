using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Interfaces.Service;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.User.Commands.UpdatePassword;

internal sealed class UpdatePasswordUserCommandHandler(
    IUserRepository userRepository,
    ILogger<UpdatePasswordUserCommandHandler> logger,
    IEmailConfirmationTokenService emailConfirmationTokenService
    ) : ICommandHandler<UpdatePasswordUserCommand, string>
{
    public async Task<ResultT<string>> Handle(
        UpdatePasswordUserCommand request, 
        CancellationToken cancellationToken)
    {
        if (request != null)
        {
            var user = await userRepository.GetByEmailAsync(request.Email!, cancellationToken);
            if (user == null)
            {
                logger.LogWarning("User with email {RequestEmail} not found", request.Email);
                
                return ResultT<string>.Failure(Error.NotFound("404", $"No user found with email {request.Email}"));
            }
            
            var codeAvailabilityResult = await emailConfirmationTokenService.IsCodeAvailableAsync(request.Code!, cancellationToken);
            if (!codeAvailabilityResult.IsSuccess)
            {
                logger.LogWarning("Code '{Code}' has already been used.", request.Code);
                
                return ResultT<string>.Failure(Error.Conflict("409", "The code has already been used."));
            }
            
            logger.LogInformation("Code '{Code}' is available.", request.Code);

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.ConfirmNewPassword);

            await userRepository.UpdatePasswordAsync(user, hashedPassword, cancellationToken);

            return ResultT<string>.Success("Password has been successfully updated.");
        }
        
        logger.LogWarning("UpdatePasswordUserCommand: Request is null.");
        
        return ResultT<string>.Failure(Error.Failure("400", "Invalid request"));
    }
}