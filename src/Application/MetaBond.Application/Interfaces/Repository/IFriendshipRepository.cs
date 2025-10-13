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
    /// Retrieves a paginated list of friendships ordered by <c>Id</c> in ascending order.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve (1-based).</param>
    /// <param name="pageSize">The number of records per page.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="PagedResult{Friendship}"/> containing friendships ordered by <c>Id</c> ascending.</returns>
    Task<PagedResult<Friendship>> OrderByIdAscAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a paginated list of friendships ordered by <c>Id</c> in descending order.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve (1-based).</param>
    /// <param name="pageSize">The number of records per page.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="PagedResult{Friendship}"/> containing friendships ordered by <c>Id</c> descending.</returns>
    Task<PagedResult<Friendship>> OrderByIdDescAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all friendships in a paginated format without additional filters.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve (1-based).</param>
    /// <param name="pageSize">The number of records per page.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A paginated result of all friendships.</returns>
    Task<PagedResult<Friendship>> GetPagedFriendshipAsync(int pageNumber, int pageSize,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves friendships created before the specified date in a paginated format.
    /// </summary>
    /// <param name="date">The cutoff date (exclusive).</param>
    /// <param name="pageNumber">The page number to retrieve (1-based).</param>
    /// <param name="pageSize">The number of records per page.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="PagedResult{Friendship}"/> containing friendships created before the given date.</returns>
    Task<PagedResult<Friendship>> GetCreatedBeforeAsync(DateTime date, int pageNumber, int pageSize,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves friendships created after the specified date in a paginated format.
    /// </summary>
    /// <param name="date">The starting date (inclusive).</param>
    /// <param name="pageNumber">The page number to retrieve (1-based).</param>
    /// <param name="pageSize">The number of records per page.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="PagedResult{Friendship}"/> containing friendships created after the given date.</returns>
    Task<PagedResult<Friendship>> GetCreatedAfterAsync(DateTime date, int pageNumber, int pageSize,
        CancellationToken cancellationToken);

    /// <summary>
    /// Counts the total number of friendships matching the specified status.
    /// </summary>
    /// <param name="status">The friendship status to filter by (e.g., Pending, Accepted).</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The number of friendships with the specified status.</returns>
    Task<int> CountByStatusAsync(Status status, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a limited number of the most recently created friendships in a paginated format.
    /// </summary>
    /// <param name="limit">The maximum number of records to return.</param>
    /// <param name="pageNumber">The page number to retrieve (1-based).</param>
    /// <param name="pageSize">The number of records per page.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="PagedResult{Friendship}"/> containing the most recently created friendships.</returns>
    Task<PagedResult<Friendship>> GetRecentlyCreatedAsync(int limit, int pageNumber, int pageSize,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves friendships filtered by a specific status in a paginated format.
    /// </summary>
    /// <param name="status">The friendship status to filter by.</param>
    /// <param name="pageNumber">The page number to retrieve (1-based).</param>
    /// <param name="pageSize">The number of records per page.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="PagedResult{Friendship}"/> containing friendships with the specified status.</returns>
    Task<PagedResult<Friendship>> GetFilterByStatusAsync(Status status, int pageNumber, int pageSize,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a friendship with its associated user details in a paginated format.
    /// </summary>
    /// <param name="friendshipId">The unique identifier of the friendship.</param>
    /// <param name="pageNumber">The page number to retrieve (1-based).</param>
    /// <param name="pageSize">The number of records per page.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="PagedResult{Friendship}"/> containing the friendship and its related users.</returns>
    Task<PagedResult<Friendship>> GetFriendshipWithUsersAsync(Guid friendshipId, int pageNumber, int pageSize,
        CancellationToken cancellationToken);
}