using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Interfaces.Repository.Account;

public interface ICommunityUserRepository : IGenericRepository<CommunityUser>
{
    /// <summary>
    /// Retrieves a paginated list of community users.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A paged result containing community users.</returns>
    Task<PagedResult<CommunityUser>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all community user records for a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A list of community user entries associated with the user.</returns>
    Task<IEnumerable<CommunityUser>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all community user records for a specific community.
    /// </summary>
    /// <param name="communityId">The unique identifier of the community.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A list of community user entries associated with the community.</returns>
    Task<IEnumerable<CommunityUser>> GetByCommunityIdAsync(Guid communityId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all communities associated with a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A list of communities that the user is a part of.</returns>
    Task<IEnumerable<CommunityUser>> GetCommunitiesByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Counts the total number of members in a specific community.
    /// </summary>
    /// <param name="communityId">The unique identifier of the community.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The total number of members in the community.</returns>
    Task<int> CountMembersByCommunityIdAsync(Guid communityId, CancellationToken cancellationToken);

    /// <summary>
    /// Removes a user from a community.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to remove.</param>
    /// <param name="communityId">The unique identifier of the community.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveAsync(Guid userId, Guid communityId, CancellationToken cancellationToken);
}