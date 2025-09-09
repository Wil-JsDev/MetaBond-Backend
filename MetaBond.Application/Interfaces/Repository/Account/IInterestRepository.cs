using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Interfaces.Repository.Account;

public interface IInterestRepository : IGenericRepository<Interest>
{
    /// <summary>
    /// Retrieves a paged list of interests that match the specified name.
    /// </summary>
    /// <param name="interestName">The name or partial name of the interest to search for.</param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A paged result containing the interests that match the search criteria.</returns>
    Task<PagedResult<Interest>> GetInterestsByNameAsync(string interestName,
        int pageNumber, int pageSize,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a paged list of all interests with the specified page number and page size.
    /// </summary>
    /// <param name="pageNumber">The current page number (starting from 1).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A paged result containing the interests for the specified page.</returns>
    Task<PagedResult<Interest>>
        GetPagedInterestAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a paged list of interests associated with a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user whose interests are being retrieved.</param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A paged result containing the interests associated with the user.</returns>
    Task<PagedResult<Interest>> GetInterestsByUserAsync(Guid userId, int pageNumber, int pageSize,
        CancellationToken cancellationToken);

    /// <summary>
    /// Checks if an interest with the specified name exists in the database.
    /// </summary>
    /// <param name="interestName">The name of the interest to check.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>True if the interest exists; otherwise, false.</returns>
    Task<bool> InterestExistsAsync(string interestName, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a paginated list of interests that belong to the specified interest category.
    /// </summary>
    /// <param name="interestCategoryId">The ID of the interest category.</param>
    /// <param name="pageNumber">The page number to retrieve (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A paged result containing the interests for the given category.</returns>
    Task<PagedResult<Interest>> GetPagedInterestByInterestCategoryIdAsync(
        Guid interestCategoryId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);
}