using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Interfaces.Repository;

/// <summary>
/// Repository interface for managing progress boards, including retrieval, pagination, and relationships with other entities.
/// </summary>
public interface IProgressBoardRepository : IGenericRepository<ProgressBoard>
{
    /// <summary>
    /// Retrieves a paginated list of progress boards.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token for the asynchronous operation.</param>
    /// <returns>A paginated result of progress boards.</returns>
    Task<PagedResult<ProgressBoard>> GetPagedBoardsAsync(int pageNumber, int pageSize,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves progress boards created after a specific date.
    /// </summary>
    /// <param name="dateTime">The minimum creation date of the boards to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token for the asynchronous operation.</param>
    /// <returns>A collection of recently created progress boards.</returns>
    Task<PagedResult<ProgressBoard>> GetRecentBoardsAsync(DateTime dateTime, int pageNumber, int pageSize,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves progress boards within a specified date range.
    /// </summary>
    /// <param name="startTime">The start date of the range.</param>
    /// <param name="endTime">The end date of the range.</param>
    /// <param name="cancellationToken">Cancellation token for the asynchronous operation.</param>
    /// <returns>A collection of progress boards created within the given date range.</returns>
    Task<PagedResult<ProgressBoard>> GetBoardsByDateRangeAsync(DateTime startTime, DateTime endTime, int pageNumber,
        int pageSize, CancellationToken cancellationToken);

    /// <summary>
    /// Counts the total number of progress boards.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the asynchronous operation.</param>
    /// <returns>The total number of progress boards.</returns>
    Task<int> CountBoardsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a progress board with its associated progress entries.
    /// </summary>
    /// <param name="id">The ID of the progress board.</param>
    /// <param name="cancellationToken">Cancellation token for the asynchronous operation.</param>
    /// <returns>A collection containing the board with its entries.</returns>
    Task<PagedResult<ProgressBoard>> GetBoardsWithEntriesAsync(Guid id, int pageNumber, int pageSize,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a progress board along with information about its author.
    /// </summary>
    /// <param name="progressBoardId">The ID of the progress board.</param>
    /// <param name="cancellationToken">Cancellation token for the asynchronous operation.</param>
    /// <returns>A collection containing the board with its author data.</returns>
    Task<PagedResult<ProgressBoard>> GetProgressBoardsWithAuthorAsync(Guid progressBoardId,
        int pageNumber, int pageSize, CancellationToken cancellationToken);
}