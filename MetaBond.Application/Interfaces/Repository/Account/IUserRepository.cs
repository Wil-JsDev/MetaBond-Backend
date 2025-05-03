using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Interfaces.Repository.Account;

public interface IUserRepository : IGenericRepository<User>
{
/// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">The email address of the user.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>The corresponding <see cref="User"/> if found; otherwise, null.</returns>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
   
    /// <summary>
    /// Retrieves a user by their username.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>The corresponding <see cref="User"/> if found; otherwise, null.</returns>
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a paginated list of users.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of users per page.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="PagedResult{User}"/> containing the users for the specified page.</returns>
    Task<PagedResult<User>> GetPagedUsersAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

    /// <summary>
    /// Searches users whose usernames match the specified keyword.
    /// </summary>
    /// <param name="keyword">The keyword to search for in usernames.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A collection of <see cref="User"/> entities that match the keyword.</returns>
    Task<IEnumerable<User>> SearchUsernameAsync(string keyword, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a paginated list of users.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of users per page.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="PagedResult{User}"/> containing the users for the specified page.</returns>
    Task<PagedResult<User>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

    /// <summary>
    /// Checks whether a user with the specified email exists.
    /// </summary>
    /// <param name="email">The email to check.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>True if the email exists; otherwise, false.</returns>
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken);

    /// <summary>
    /// Checks whether a user with the specified username exists.
    /// </summary>
    /// <param name="username">The username to check.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>True if the username exists; otherwise, false.</returns>
    Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken);

    /// <summary>
    /// Checks if a user's email account has been confirmed.
    /// </summary>
    /// <param name="userId">The userId of the user to check</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>True if the account is confirmed; otherwise, false.</returns>
    Task<bool> IsAccountConfirmedAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Confirms the email account of a user by updating their confirmation status.
    /// </summary>
    /// <param name="userId">The ID of the user to confirm.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    Task ConfirmAccountAsync(Guid userId, CancellationToken cancellationToken);
}