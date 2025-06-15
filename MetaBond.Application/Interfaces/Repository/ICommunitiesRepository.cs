using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;
using System.Linq.Expressions;

namespace MetaBond.Application.Interfaces.Repository;

/// <summary>
/// Repository interface for accessing and managing community entities.
/// Inherits from the generic repository interface.
/// </summary>
public interface ICommunitiesRepository : IGenericRepository<Communities>
{
    /// <summary>
    /// Retrieves a paginated list of communities.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageZize">The size of the page (number of items).</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A paginated result of communities.</returns>
    Task<PagedResult<Communities>> GetPagedCommunitiesAsync(int pageNumber, int pageZize, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves communities that match the specified filter predicate.
    /// </summary>
    /// <param name="predicate">Filter function to apply to communities.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A collection of communities matching the filter.</returns>
    Task<IEnumerable<Communities>> GetByFilterAsync(Func<Communities, bool> predicate, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves posts and events associated with a specific community.
    /// </summary>
    /// <param name="communitieId">The ID of the community.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A collection of communities including their posts and events.</returns>
    Task<IEnumerable<Communities>> GetPostsAndEventsByCommunityIdAsync(Guid communitieId, CancellationToken cancellationToken);

    /// <summary>
    /// Validates whether any community satisfies the given condition.
    /// </summary>
    /// <param name="predicate">A condition expressed as a LINQ expression.</param>
    /// <returns>True if any community matches the condition; otherwise, false.</returns>
    Task<bool> ValidateAsync(Expression<Func<Communities, bool>> predicate);
}
