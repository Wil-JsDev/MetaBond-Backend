using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;


namespace MetaBond.Application.Interfaces.Repository;

/// <summary>
/// Defines repository operations specific to the <see cref="Posts"/> entity,
/// focusing on queries related to communities and users.
/// Inherits from the generic repository interface.
/// </summary>
public interface IPostsRepository : IGenericRepository<Posts>
{
    /// <summary>
    /// Retrieves a paginated list of all posts.
    /// </summary>
    /// <param name="pageNumber">The number of the page to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A paginated result (<see cref="PagedResult{T}"/>) of all posts.</returns>
    Task<PagedResult<Posts>> GetPagedPostsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a paginated list of posts from a specific community, filtered by a title search.
    /// </summary>
    /// <param name="communitiesId">The ID of the community.</param>
    /// <param name="title">The title or keyword to filter by.</param>
    /// <param name="pageNumber">The number of the page to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A paginated result of posts matching the filter criteria in the specified community.</returns>
    Task<PagedResult<Posts>> GetFilterByTitleAsync(Guid communitiesId, string title,
        int pageNumber, int pageSize,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a specific post by its ID, including its associated community data.
    /// Note: The result is returned within a <see cref="PagedResult{T}"/> wrapper.
    /// </summary>
    /// <param name="id">The ID of the post to retrieve.</param>
    /// <param name="pageNumber">The page number (typically 1 for a single ID lookup).</param>
    /// <param name="pageSize">The page size (typically 1 for a single ID lookup).</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A paginated result containing the post with its related community, if found.</returns>
    Task<PagedResult<Posts>> GetPostsByIdWithCommunitiesAsync(Guid id, int pageNumber, int pageSize,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the 10 most recent posts from a specific community.
    /// </summary>
    /// <param name="communitiesId">The ID of the community.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A collection of the 10 most recent posts.</returns>
    Task<IEnumerable<Posts>> FilterTop10RecentPostsAsync(Guid communitiesId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a paginated list of recent posts from a community, ordered by creation date descending.
    /// </summary>
    /// <param name="communitiesId">The ID of the community.</param>
    /// <param name="pageNumber">The number of the page to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A paginated result of recent posts from the specified community.</returns>
    Task<PagedResult<Posts>> FilterRecentPostsByCountAsync(Guid communitiesId,
        int pageNumber, int pageSize, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a specific post by its ID, including its associated author (user) data.
    /// Note: The result is returned within a <see cref="PagedResult{T}"/> wrapper.
    /// </summary>
    /// <param name="postsId">The ID of the post.</param>
    /// <param name="pageNumber">The page number (typically 1 for a single ID lookup).</param>
    /// <param name="pageSize">The page size (typically 1 for a single ID lookup).</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A paginated result containing the post and its author details, if found.</returns>
    Task<PagedResult<Posts>> GetPostWithAuthorAsync(Guid postsId, int pageNumber, int pageSize,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a paginated list of posts created by a specific user within a specific community.
    /// </summary>
    /// <param name="userId">The ID of the user (author).</param>
    /// <param name="communitiesId">The ID of the community.</param>
    /// <param name="pageNumber">The number of the page to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A paginated result of posts matching the specified user and community.</returns>
    Task<PagedResult<Posts>> GetPagedPostsByUserAndCommunitiesAsync(Guid userId, Guid communitiesId, int pageNumber,
        int pageSize, CancellationToken cancellationToken);
}