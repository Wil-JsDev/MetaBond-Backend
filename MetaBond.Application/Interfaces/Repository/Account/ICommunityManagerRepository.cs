using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Interfaces.Repository.Account;

public interface ICommunityManagerRepository : IGenericRepository<CommunityManager>
{
    /// <summary>
    /// Retrieves a paginated list of community managers.
    /// </summary>
    /// <param name="page">The page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A paged result containing community managers.</returns>
    Task<PagedResult<CommunityManager>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken);
    
    /// <summary>
    /// Determines if a specific user is a manager of a given community.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="communityId">The unique identifier of the community.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>True if the user is a manager of the community; otherwise, false.</returns>
    Task<bool> IsUserCommunityManagerAsync(Guid userId, Guid communityId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all community manager records for a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A list of community manager entries associated with the user.</returns>
    Task<List<CommunityManager>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all community manager records for a specific community.
    /// </summary>
    /// <param name="communityId">The unique identifier of the community.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A list of community manager entries associated with the community.</returns>
    Task<List<CommunityManager>> GetByCommunityIdAsync(Guid communityId, CancellationToken cancellationToken);

    /// <summary>
    /// Counts the total number of community managers in a specific community.
    /// </summary>
    /// <param name="communityId">The unique identifier of the community.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The total number of managers assigned to the community.</returns>
    Task<int> CountCommunityManagersAsync(Guid communityId, CancellationToken cancellationToken);

    /// <summary>
    /// Checks if any manager is currently assigned to a specific community.
    /// </summary>
    /// <param name="communityId">The unique identifier of the community.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>True if at least one manager is assigned; otherwise, false.</returns>
    Task<bool> IsManagerAssignedToCommunityAsync(Guid communityId, CancellationToken cancellationToken);

    /// <summary>
    /// Checks if a manager exists by their unique identifier.
    /// </summary>
    /// <param name="managerId">The unique identifier of the community manager.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>True if the manager exists; otherwise, false.</returns>
    Task<bool> DoesManagerExistAsync(Guid managerId, CancellationToken cancellationToken);

}