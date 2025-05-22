using MetaBond.Application.Pagination;
using MetaBond.Domain;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Interfaces.Repository;

/// <summary>
/// Repository interface for managing friendship relationships between users.
/// Inherits from the generic repository interface.
/// </summary>
public interface IFriendshipRepository : IGenericRepository<Friendship>
{
    /// <summary>
    /// Retrieves all friendship records ordered by ID in ascending order.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A collection of friendships ordered by ID ascending.</returns>
    Task<IEnumerable<Friendship>> OrderByIdAscAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all friendship records ordered by ID in descending order.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A collection of friendships ordered by ID descending.</returns>
    Task<IEnumerable<Friendship>> OrderByIdDescAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a paginated list of friendship records.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The size of each page.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A paginated result of friendships.</returns>
    Task<PagedResult<Friendship>> GetPagedFriendshipAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves friendships that were created before the specified date.
    /// </summary>
    /// <param name="date">The cutoff date.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A collection of friendships created before the given date.</returns>
    Task<IEnumerable<Friendship>> GetCreatedBeforeAsync(DateTime date, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves friendships that were created after the specified date.
    /// </summary>
    /// <param name="date">The starting date.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A collection of friendships created after the given date.</returns>
    Task<IEnumerable<Friendship>> GetCreatedAfterAsync(DateTime date, CancellationToken cancellationToken);

    /// <summary>
    /// Counts the number of friendships by their status.
    /// </summary>
    /// <param name="status">The status to filter by (e.g., Pending, Accepted).</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>The number of friendships with the given status.</returns>
    Task<int> CountByStatusAsync(Status status, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a limited number of the most recently created friendships.
    /// </summary>
    /// <param name="limit">The maximum number of results to return.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A collection of recently created friendships.</returns>
    Task<IEnumerable<Friendship>> GetRecentlyCreatedAsync(int limit, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves friendships filtered by a specific status.
    /// </summary>
    /// <param name="status">The status to filter by.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A collection of friendships with the specified status.</returns>
    Task<IEnumerable<Friendship>> GetFilterByStatusAsync(Status status, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a friendship with related user data.
    /// </summary>
    /// <param name="friendshipId">The ID of the friendship.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A collection of friendships including user details.</returns>
    Task<IEnumerable<Friendship>> GetFriendshipWithUsersAsync(Guid friendshipId, CancellationToken cancellationToken);
}
