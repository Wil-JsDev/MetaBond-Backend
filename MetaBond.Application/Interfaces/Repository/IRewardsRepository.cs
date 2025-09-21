using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Interfaces.Repository;

/// <summary>
/// Repository interface for managing rewards-related operations,
/// including pagination, filtering by date, and retrieving related user data.
/// </summary>
public interface IRewardsRepository : IGenericRepository<Rewards>
{
    /// <summary>
    /// Retrieves a paginated collection of rewards.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A paged result containing the rewards for the specified page.</returns>
    Task<PagedResult<Rewards>> GetPagedRewardsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the most recently created reward.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The latest reward created in the system.</returns>
    Task<Rewards> GetMostRecentRewardAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a paginated collection of rewards created within a specified date range.
    /// </summary>
    /// <param name="startTime">The start of the date range (inclusive).</param>
    /// <param name="endTime">The end of the date range (inclusive).</param>
    /// <param name="pageNumber">The page number to retrieve (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A paged result containing rewards created in the given date range.</returns>
    Task<PagedResult<Rewards>> GetRewardsByDateRangeAsync(DateTime startTime, DateTime endTime,
        int pageNumber, int pageSize, CancellationToken cancellationToken);

    /// <summary>
    /// Counts the total number of rewards in the system.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The total count of rewards.</returns>
    Task<int> CountRewardsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a paginated collection of rewards ordered by points in descending order.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A paged result containing the top rewards sorted by points.</returns>
    Task<PagedResult<Rewards>> GetTopRewardsByPointsAsync(int pageNumber, int pageSize,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a paginated collection of users associated with a specific reward.
    /// </summary>
    /// <param name="rewardId">The unique identifier of the reward.</param>
    /// <param name="pageNumber">The page number to retrieve (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A paged result containing the users linked to the specified reward.</returns>
    Task<PagedResult<Rewards>> GetUsersByRewardIdAsync(Guid rewardId, int pageNumber, int pageSize,
        CancellationToken cancellationToken);
}