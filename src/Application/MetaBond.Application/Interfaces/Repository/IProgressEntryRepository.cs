using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Interfaces.Repository;

/// <summary>
/// Repository interface for managing progress entries within a progress board,
/// including filtering, ordering, and retrieving related data.
/// </summary>
public interface IProgressEntryRepository : IGenericRepository<ProgressEntry>
{
    /// <summary>
    /// Retrieves a paginated list of progress entries.
    /// </summary>
    /// <param name="pageSize">The number of entries per page.</param>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token for the asynchronous operation.</param>
    /// <returns>A paginated result of progress entries.</returns>
    Task<PagedResult<ProgressEntry>> GetPagedProgressEntryAsync(
        int pageSize,
        int pageNumber,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves progress entries of a board ordered by ID.
    /// </summary>
    /// <param name="progressBoardId">The ID of the progress board.</param>
    /// <param name="pageSize">The number of entries per page.</param>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token for the asynchronous operation.</param>
    /// <returns>A paginated collection of progress entries ordered by ID.</returns>
    Task<PagedResult<ProgressEntry>> GetOrderByIdAsync(
        Guid progressBoardId,
        int pageSize,
        int pageNumber,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves progress entries within a specific date range for a given board.
    /// </summary>
    /// <param name="progressBoardId">The ID of the progress board.</param>
    /// <param name="startTime">Start of the date range.</param>
    /// <param name="endTime">End of the date range.</param>
    /// <param name="pageSize">The number of entries per page.</param>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token for the asynchronous operation.</param>
    /// <returns>A paginated collection of progress entries in the date range.</returns>
    Task<PagedResult<ProgressEntry>> GetEntriesByDateRangeAsync(
        Guid progressBoardId,
        DateTime startTime,
        DateTime endTime,
        int pageSize,
        int pageNumber,
        CancellationToken cancellationToken);

    /// <summary>
    /// Counts the total number of progress entries for a specific board.
    /// </summary>
    /// <param name="id">The ID of the progress board.</param>
    /// <param name="cancellationToken">Cancellation token for the asynchronous operation.</param>
    /// <returns>The total number of progress entries.</returns>
    Task<int> CountEntriesByBoardIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the most recent progress entries of a board.
    /// </summary>
    /// <param name="progressBoardId">The ID of the progress board.</param>
    /// <param name="pageSize">The number of entries per page.</param>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token for the asynchronous operation.</param>
    /// <returns>A paginated collection of recent progress entries.</returns>
    Task<PagedResult<ProgressEntry>> GetRecentEntriesAsync(
        Guid progressBoardId,
        int pageSize,
        int pageNumber,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves progress entries of a board ordered by description.
    /// </summary>
    /// <param name="progressBoardId">The ID of the progress board.</param>
    /// <param name="pageSize">The number of entries per page.</param>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token for the asynchronous operation.</param>
    /// <returns>A paginated collection of progress entries ordered by description.</returns>
    Task<PagedResult<ProgressEntry>> GetOrderByDescriptionAsync(
        Guid progressBoardId,
        int pageSize,
        int pageNumber,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a progress entry with its related progress board.
    /// </summary>
    /// <param name="progressEntry">The ID of the progress entry.</param>
    /// <param name="cancellationToken">Cancellation token for the asynchronous operation.</param>
    /// <returns>The progress entry with its board.</returns>
    Task<PagedResult<ProgressEntry>> GetByIdProgressEntryWithProgressBoard(
        Guid progressEntry,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves progress entries of a board including author information.
    /// </summary>
    /// <param name="progressBoardId">The ID of the progress board.</param>
    /// <param name="pageSize">The number of entries per page.</param>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token for the asynchronous operation.</param>
    /// <returns>A paginated collection of progress entries with author details.</returns>
    Task<PagedResult<ProgressEntry>> GetProgressEntriesWithAuthorsAsync(
        Guid progressBoardId,
        int pageSize,
        int pageNumber,
        CancellationToken cancellationToken);
}