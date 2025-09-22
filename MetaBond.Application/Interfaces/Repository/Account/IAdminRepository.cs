using MetaBond.Application.Pagination;
using MetaBond.Domain;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Interfaces.Repository.Account;

/// <summary>
/// Repository interface for Admin entity, providing methods for admin-specific queries and operations.
/// </summary>
public interface IAdminRepository : IGenericRepository<Admin>
{
    /// <summary>
    /// Gets a paged list of Admins.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paged result of Admins.</returns>
    Task<PagedResult<Admin>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

    /// <summary>
    /// Gets an Admin by email.
    /// </summary>
    /// <param name="email">The email to search for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The Admin if found, otherwise null.</returns>
    Task<Admin?> GetByEmailAsync(string email, CancellationToken cancellationToken);

    /// <summary>
    /// Gets an Admin by username.
    /// </summary>
    /// <param name="username">The username to search for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The Admin if found, otherwise null.</returns>
    Task<Admin?> GetByUsernameAsync(string username, CancellationToken cancellationToken);

    /// <summary>
    /// Checks if the email is not used by any Admin.
    /// </summary>
    /// <param name="email">The email to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the email is unused, otherwise false.</returns>
    Task<bool> IsEmailUnusedAsync(string email, CancellationToken cancellationToken);

    /// <summary>
    /// Checks if a username exists among Admins.
    /// </summary>
    /// <param name="username">The username to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the username exists, otherwise false.</returns>
    Task<bool> ExistsUsernameAsync(string username, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a paged list of Users filtered by their account status.
    /// </summary>
    /// <param name="statusAccount">The status to filter users by (e.g., Active, Inactive, Banned).</param>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paged result of Users with the specified status.</returns>
    Task<PagedResult<User>> GetPagedUserStatusAsync(StatusAccount statusAccount, int pageNumber, int pageSize,
        CancellationToken cancellationToken);

    Task<User> BanUserAsync(Guid userId, CancellationToken cancellationToken);

    Task<User> UnbanUserAsync(Guid userId, CancellationToken cancellationToken);
}