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
    /// Retrieves a paginated list of rewards.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageZize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token for the asynchronous operation.</param>
    /// <returns>A paged result of rewards.</returns>
    Task<PagedResult<Rewards>> GetPagedRewardsAsync(int pageNumber, int pageZize, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the most recently created reward.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the asynchronous operation.</param>
    /// <returns>The most recent reward.</returns>
    Task<Rewards> GetMostRecentRewardAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all rewards created within the specified date range.
    /// </summary>
    /// <param name="startTime">The start of the date range.</param>
    /// <param name="endTime">The end of the date range.</param>
    /// <param name="cancellationToken">Cancellation token for the asynchronous operation.</param>
    /// <returns>A collection of rewards within the date range.</returns>
    Task<IEnumerable<Rewards>> GetRewardsByDateRangeAsync(DateTime startTime, DateTime endTime,
        CancellationToken cancellationToken);

    /// <summary>
    /// Counts the total number of rewards.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the asynchronous operation.</param>
    /// <returns>The total number of rewards.</returns>
    Task<int> CountRewardsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the top rewards sorted by points in descending order.
    /// </summary>
    /// <param name="topCount">The number of top rewards to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token for the asynchronous operation.</param>
    /// <returns>A collection of the top rewards by points.</returns>
    Task<IEnumerable<Rewards>> GetTopRewardsByPointsAsync(int topCount, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves users associated with a specific reward.
    /// </summary>
    /// <param name="rewardId">The unique identifier of the reward.</param>
    /// <param name="cancellationToken">Cancellation token for the asynchronous operation.</param>
    /// <returns>A collection of users who received the specified reward.</returns>
    Task<IEnumerable<Rewards>> GetUsersByRewardIdAsync(Guid rewardId, CancellationToken cancellationToken);
}