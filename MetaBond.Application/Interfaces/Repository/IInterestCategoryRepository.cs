using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Interfaces.Repository;

/// <summary>
/// Repository interface for managing <see cref="InterestCategory"/> entities.
/// </summary>
public interface IInterestCategoryRepository : IGenericRepository<InterestCategory>
{
    /// <summary>
    /// Checks if a category with the specified name exists.
    /// </summary>
    /// <param name="name">The name of the category to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the name exists; otherwise, false.</returns>
    Task<bool> ExistsNameAsync(string name, CancellationToken cancellationToken);

    /// <summary>
    /// Checks if a category name exists excluding a specific category Id (useful for updates).
    /// </summary>
    /// <param name="name">The category name to check.</param>
    /// <param name="excludeId">The Id to exclude from the check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the name exists for another category; otherwise, false.</returns>
    Task<bool> ExistsNameExceptIdAsync(string name, Guid excludeId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a category by its name.
    /// </summary>
    /// <param name="name">The name of the category.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The category if found; otherwise, null.</returns>
    Task<InterestCategory?> GetByNameAsync(string name, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a paginated list of categories.
    /// </summary>
    /// <param name="pageNumber">The page number (starting at 1).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated result of <see cref="InterestCategory"/> entities.</returns>
    Task<PagedResult<InterestCategory>>
        GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
}