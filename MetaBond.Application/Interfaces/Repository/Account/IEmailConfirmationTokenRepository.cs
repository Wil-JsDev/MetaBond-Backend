using MetaBond.Domain.Models;

namespace MetaBond.Application.Interfaces.Repository.Account;

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
    /// <param name="token">The unique identifier of the token.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    Task DeleteToken(EmailConfirmationToken token, CancellationToken cancellationToken);

    /// <summary>
    /// Checks whether an email confirmation token exists.
    /// </summary>
    /// <param name="token">The token string to check.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>True if the token exists; otherwise, false.</returns>
    Task<bool> ExistsToken(string token, CancellationToken cancellationToken);

}