using MetaBond.Domain.Models;

namespace MetaBond.Application.Interfaces.Repository.Account;

public interface IModeratorCommunityRepository 
{
    /// <summary>
    /// Checks if a moderator is assigned to a specific community.
    /// </summary>
    /// <param name="moderatorId">The unique identifier of the moderator.</param>
    /// <param name="communityId">The unique identifier of the community.</param>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>True if the moderator is part of the community; otherwise, false.</returns>
    Task<bool> IsModeratorOfCommunityAsync(Guid moderatorId, Guid communityId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all communities associated with a specific moderator.
    /// </summary>
    /// <param name="moderatorId">The unique identifier of the moderator.</param>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>A list of communities managed by the moderator.</returns>
    Task<IEnumerable<Communities>> GetCommunitiesByModeratorIdAsync(Guid moderatorId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all ModeratorCommunity relationships for a specific moderator.
    /// </summary>
    /// <param name="moderatorId">The unique identifier of the moderator.</param>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>A list of ModeratorCommunity relationships.</returns>
    Task<IEnumerable<ModeratorCommunity>> GetModeratorCommunitiesAsync(Guid moderatorId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a moderator along with detailed information such as user data and assigned communities.
    /// </summary>
    /// <param name="moderatorId">The unique identifier of the moderator.</param>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>The moderator with related user and communities data.</returns>
    Task<Moderator?> GetModeratorWithDetailsAsync(Guid moderatorId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Removes the relationship between a moderator and a community.
    /// </summary>
    /// <param name="moderatorId">The unique identifier of the moderator.</param>
    /// <param name="communityId">The unique identifier of the community.</param>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    Task RemoveModeratorFromCommunityAsync(Guid moderatorId, Guid communityId, CancellationToken cancellationToken);

    /// <summary>
    /// Checks if a moderator-community relationship already exists.
    /// </summary>
    /// <param name="moderatorId">The unique identifier of the moderator.</param>
    /// <param name="communityId">The unique identifier of the community.</param>
    /// <param name="cancellationToken">The cancellation token for the asynchronous operation.</param>
    /// <returns>True if the relationship exists; otherwise, false.</returns>
    Task<bool> ModeratorCommunityExistsAsync(Guid moderatorId, Guid communityId, CancellationToken cancellationToken);

}