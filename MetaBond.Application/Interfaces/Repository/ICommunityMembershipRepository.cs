using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Interfaces.Repository;

/// <summary>
/// Repository interface for managing community memberships,
/// providing methods to query and update user participation within communities.
/// </summary>
public interface ICommunityMembershipRepository : IGenericRepository<CommunityMembership>
{
    /// <summary>
    /// Retrieves a paged list of all members belonging to a specific community.
    /// </summary>
    /// <param name="communityId">The unique identifier of the community.</param>
    /// <param name="pageNumber">The current page number of the result set.</param>
    /// <param name="pageSize">The number of items to return per page.</param>
    /// <param name="cancellationToken">Token for canceling the asynchronous operation.</param>
    Task<PagedResult<CommunityMembership>> GetCommunityMembersAsync(Guid communityId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a paged list of all communities that a given user is a member of.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="pageNumber">The current page number of the result set.</param>
    /// <param name="pageSize">The number of items to return per page.</param>
    /// <param name="cancellationToken">Token for canceling the asynchronous operation.</param>
    Task<PagedResult<CommunityMembership>> GetUserCommunitiesAsync(Guid userId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    /// <summary>
    /// Checks whether a user is already a member of a specific community.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="communityId">The unique identifier of the community.</param>
    /// <param name="cancellationToken">Token for canceling the asynchronous operation.</param>
    Task<bool> IsUserMemberAsync(Guid userId, Guid communityId, CancellationToken cancellationToken);

    /// <summary>
    /// Updates the role of a user within a specific community.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="communityId">The unique identifier of the community.</param>
    /// <param name="role">The new role to assign to the user (e.g., Member, Admin, Moderator).</param>
    /// <param name="cancellationToken">Token for canceling the asynchronous operation.</param>
    Task UpdateUserRoleAsync(Guid userId, Guid communityId, string role, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a new membership record when a user joins a community.
    /// </summary>
    /// <param name="entity">The community membership entity containing user and community details.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>
    /// The created <see cref="CommunityMembership"/> entity with assigned identifiers and persistence state.
    /// </returns>
    Task<CommunityMembership> JoinCommunityAsync(CommunityMembership entity, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the role of a specific user within a given community.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="communityId">The unique identifier of the community.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>
    /// A string representing the user's role within the community, 
    /// or <c>null</c> if the user is not a member of the community.
    /// </returns>
    Task<string?> GetUserRoleAsync(Guid userId, Guid communityId, CancellationToken cancellationToken);

    Task<CommunityMembership> LeaveCommunityAsync(Guid userId, Guid communityId,
        CancellationToken cancellationToken);
}