using MetaBond.Application.DTOs.Email;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Interfaces.Service;
using MetaBond.Application.Utils;
using MetaBond.Domain.Models;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Services;

public class EmailConfirmationTokenService(
    IEmailConfirmationTokenRepository emailConfirmationTokenRepository,
    IUserRepository userRepository,
    ILogger<EmailConfirmationTokenService> logger
    ): IEmailConfirmationTokenService
{
    public async Task<ResultT<string>> GenerateTokenAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            logger.LogWarning("GenerateTokenAsync: User with ID '{UserId}' not found.", userId);
            
            return ResultT<string>.Failure(Error.NotFound("404", "User not found"));
        }
        
        if (user.IsEmailConfirmed)
        {
            logger.LogWarning("GenerateTokenAsync: Email already confirmed for user ID '{UserId}'.", userId);
            
            return ResultT<string>.Failure(Error.Failure("409", "Email already confirmed."));
        }

        string token = TokenGenerator.GenerateNumericToken();

        EmailConfirmationToken confirmationToken = new()
        {
            UserId = user.Id,
            Code = token,
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };
        
       await emailConfirmationTokenRepository.CreateToken(confirmationToken,cancellationToken);
       
       logger.LogInformation("GenerateTokenAsync: Confirmation token created successfully for user ID '{UserId}'.", userId);
       
        return ResultT<string>.Success(token);
    }

    public async Task<ResultT<EmailConfirmationTokenDTos>> GetByTokenAsync(string token, CancellationToken cancellationToken)
    {
        var emailConfirmationToken = await emailConfirmationTokenRepository.FindByToken(token, cancellationToken);

        if (emailConfirmationToken == null)
        {
            logger.LogWarning("GetByTokenAsync: No token found with code '{Token}'.", token);
            return ResultT<EmailConfirmationTokenDTos>.Failure(Error.NotFound("404", "Token not found"));
        }

        EmailConfirmationTokenDTos confirmationToken = new
        (
            EmailConfirmationTokenId: emailConfirmationToken.Id,
            UserId: emailConfirmationToken.UserId,
            Token: emailConfirmationToken.Code!,
            IsUsed: emailConfirmationToken.IsUsed,
            ExpiresAt: emailConfirmationToken.ExpiresAt
        );

        logger.LogInformation("GetByTokenAsync: Token '{Token}' retrieved successfully for user ID '{UserId}'.",
            emailConfirmationToken.Code, emailConfirmationToken.UserId);

        return ResultT<EmailConfirmationTokenDTos>.Success(confirmationToken);
    }

    public async Task<Result> DeleteAsync(Guid emailConfirmationTokenId, CancellationToken cancellationToken)
    {
        var emailConfirmationToken = await emailConfirmationTokenRepository.GetByIdAsync(emailConfirmationTokenId, cancellationToken);
    
        if (emailConfirmationToken == null)
        {
            logger.LogWarning("DeleteAsync: No EmailConfirmationToken found with ID '{EmailConfirmationTokenId}'.", emailConfirmationTokenId);
            return Result.Failure(Error.NotFound("404", "EmailConfirmationTokenId not found"));
        }

        await emailConfirmationTokenRepository.DeleteToken(emailConfirmationToken, cancellationToken);
    
        logger.LogInformation("DeleteAsync: EmailConfirmationToken with ID '{EmailConfirmationTokenId}' deleted successfully.", emailConfirmationTokenId);

        return Result.Success();
    }
    
    public async Task<Result> IsCodeAvailableAsync(string code, CancellationToken cancellationToken)
    {
        var codeIsUsed = await emailConfirmationTokenRepository.IsCodeUnusedAsync(code, cancellationToken);
        if (codeIsUsed)
        {
            logger.LogWarning("Code '{Code}' has already been used.", code);
            
            return Result.Failure(Error.Conflict("409", "The code has already been used."));
        }
        
        logger.LogInformation("Code '{Code}' is valid and available.", code);
        
        return Result.Success();
    }

    public async Task<Result> ConfirmAccountAsync(Guid userId, string token, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            logger.LogWarning("ConfirmAccountAsync: User with ID '{UserId}' not found.", userId);
            
            return Result.Failure(Error.NotFound("404", "User not found"));
        }

        var emailConfirmationToken = await emailConfirmationTokenRepository.FindByToken(token, cancellationToken);
        if (emailConfirmationToken == null)
        {
            logger.LogWarning("ConfirmAccountAsync: Token '{Token}' not found.", token);
            
            return Result.Failure(Error.NotFound("404", "Token not found"));
        }
        
        if (emailConfirmationToken.UserId != userId)
        {
            logger.LogWarning("ConfirmAccountAsync: Token '{Token}' does not belong to user '{UserId}'.", token, userId);
            
            return Result.Failure(Error.Failure("403", "Token does not belong to user."));
        }
        
        if (emailConfirmationToken.IsUsed)
        {
            logger.LogWarning("ConfirmAccountAsync: Token '{Token}' has already been used.", token);
            
            return Result.Failure(Error.Failure("400", "Token already used."));
        }
        
        var isValidToken = await emailConfirmationTokenRepository.IsValidTokenAsync(token, cancellationToken);
        if (!isValidToken)
        {
            logger.LogWarning("ConfirmAccountAsync: Token '{Token}' is invalid (either expired or already used).", token);
            
            return Result.Failure(Error.Failure("400", "Token is invalid. It may have expired or has already been used."));
        }

        await emailConfirmationTokenRepository.MarkTokenAsUsedAsync(emailConfirmationToken.Code!, cancellationToken);
        
        user.IsEmailConfirmed = true;
        await userRepository.UpdateAsync(user, cancellationToken);
        
        logger.LogInformation("ConfirmAccountAsync: User '{UserId}' confirmed their email successfully.", userId);
        
        return Result.Success();
    }
    
}