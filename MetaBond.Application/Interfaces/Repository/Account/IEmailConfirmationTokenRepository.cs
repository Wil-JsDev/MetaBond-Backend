using MetaBond.Domain.Models;

namespace MetaBond.Application.Interfaces.Repository.Account;

/// <summary>
/// Provides methods for managing email confirmation tokens.
/// </summary>
public interface IEmailConfirmationTokenRepository
{
    /// <summary>
    /// Generates and stores a new email confirmation token.
    /// </summary>
    /// <param name="token">The email confirmation token to be generated and saved.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    Task CreateToken(EmailConfirmationToken token, CancellationToken cancellationToken);
    
    /// <summary>
    /// Retrieves an email confirmation token by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the email confirmation token.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>The <see cref="EmailConfirmationToken"/> corresponding to the specified ID.</returns>
    Task<EmailConfirmationToken?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Finds an email confirmation token by its token string.
    /// </summary>
    /// <param name="token">The token string used to search.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>The corresponding <see cref="EmailConfirmationToken"/> if found; otherwise, null.</returns>
    Task<EmailConfirmationToken?> FindByToken(string token, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes an email confirmation token using its unique identifier.
    /// </summary>
    /// <param name="token">The email confirmation token entity to be deleted.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    Task DeleteToken(EmailConfirmationToken token, CancellationToken cancellationToken);

    /// <summary>
    /// Checks whether an email confirmation token exists.
    /// </summary>
    /// <param name="token">The token string to check.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>True if the token exists; otherwise, false.</returns>
    Task<bool> ExistsToken(string token, CancellationToken cancellationToken);

    /// <summary>
    /// Validates whether a token exists, is not expired, and has not been used.
    /// </summary>
    /// <param name="tokenCode">The token string to validate.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>True if the token is valid; otherwise, false.</returns>
    Task<bool> IsValidTokenAsync(string tokenCode, CancellationToken cancellationToken);

    /// <summary>
    /// Marks a specific email confirmation token as used.
    /// </summary>
    /// <param name="tokenCode">The token string to mark as used.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    Task MarkTokenAsUsedAsync(string tokenCode, CancellationToken cancellationToken);

    
    /// <summary>
    /// Checks if the specified email confirmation code exists and has not been used.
    /// </summary>
    /// <param name="code">The email confirmation code to validate.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains 
    /// <c>true</c> if the code exists and has not been used; otherwise, <c>false</c>.
    /// </returns>
    Task<bool> IsCodeUnusedAsync(string code, CancellationToken cancellationToken);
}