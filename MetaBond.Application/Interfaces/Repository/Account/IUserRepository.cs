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
    /// Retrieves a user along with their sent and received friendship requests,
    /// including the details of the other users involved in each friendship.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation if needed.</param>
    /// <returns>A <see cref="User"/> object with <c>SentRequests</c> and <c>ReceivedRequests</c> included, or <c>null</c> if the user is not found.</returns>
    Task<User?> GetUserWithFriendshipsAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Checks whether the user's email has been confirmed.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>True if the user's email is confirmed; otherwise, false.</returns>
    Task<bool> IsEmailConfirmedAsync(Guid userId, CancellationToken cancellationToken);
}