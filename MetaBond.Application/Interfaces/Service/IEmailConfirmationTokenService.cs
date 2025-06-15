using MetaBond.Application.DTOs.Email;
using MetaBond.Application.Utils;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Interfaces.Service;

/// <summary>
/// Service interface for managing email confirmation tokens and account confirmation process.
/// </summary>
public interface IEmailConfirmationTokenService
{
    /// <summary>
    /// Generates a new email confirmation token for the specified user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user for whom to generate the token.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>
    /// A result containing the generated token string if successful, or an error if the user is not found or email is already confirmed.
    /// </returns>
    Task<ResultT<string>> GenerateTokenAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the email confirmation token details by the token string.
    /// </summary>
    /// <param name="token">The email confirmation token string.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>
    /// A result containing the token data transfer object if found; otherwise, an error result.
    /// </returns>
    Task<ResultT<EmailConfirmationTokenDTos>> GetByTokenAsync(string token, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes an email confirmation token by its unique identifier.
    /// </summary>
    /// <param name="emailConfirmationTokenId">The unique identifier of the email confirmation token to delete.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>
    /// A result indicating whether the deletion was successful or failed due to the token not being found.
    /// </returns>
    Task<Result> DeleteAsync(Guid emailConfirmationTokenId, CancellationToken cancellationToken);

    /// <summary>
    /// Confirms the user's account by validating the given token and updating the user's confirmation status.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose account is to be confirmed.</param>
    /// <param name="token">The email confirmation token to validate.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>
    /// A result indicating whether the confirmation was successful or failed due to invalid token, user not found, or token already used.
    /// </returns>
    Task<Result> ConfirmAccountAsync(Guid userId, string token, CancellationToken cancellationToken);
    
    /// <summary>
    /// Verifies whether the specified email confirmation code is available (i.e., exists and has not been used).
    /// </summary>
    /// <param name="code">The email confirmation code to check.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a <see cref="Result"/>
    /// indicating success if the code is available, or a failure with a conflict error if the code has already been used.
    /// </returns>
    Task<Result> IsCodeAvailableAsync(string code, CancellationToken cancellationToken);
}
