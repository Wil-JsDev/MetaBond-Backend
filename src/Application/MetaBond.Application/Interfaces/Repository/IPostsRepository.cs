using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;


namespace MetaBond.Application.Interfaces.Repository;

/// <summary>
/// Repository interface for managing post records within communities.
/// Inherits from the generic repository interface.
/// </summary>
public interface IPostsRepository : IGenericRepository<Posts>
{
    /// <summary>
    /// Retrieves a paginated list of posts.
    /// </summary>
    /// <param name="pageNumber">The number of the page to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A paginated result of posts.</returns>
    Task<PagedResult<Posts>> GetPagedPostsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves posts from a specific community filtered by title.
    /// </summary>
    /// <param name="communitiesId">The ID of the community.</param>
    /// <param name="title">The title or keyword to filter by.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A collection of posts matching the title in the specified community.</returns>
    Task<PagedResult<Posts>> GetFilterByTitleAsync(Guid communitiesId, string title,
        int pageNumber, int pageSize,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a post by ID including its associated community data.
    /// </summary>
    /// <param name="id">The ID of the post.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A collection containing the post with its related community information.</returns>
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
    /// Retrieves a limited number of the most recent posts from a community.
    /// </summary>
    /// <param name="communitiesId">The ID of the community.</param>
    /// <param name="topCount">The number of recent posts to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A collection of the most recent posts up to the specified count.</returns>
    Task<PagedResult<Posts>> FilterRecentPostsByCountAsync(Guid communitiesId,
        int pageNumber, int pageSize, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a post with its associated author data.
    /// </summary>
    /// <param name="postsId">The ID of the post.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A collection containing the post and its author details.</returns>
    Task<PagedResult<Posts>> GetPostWithAuthorAsync(Guid postsId, int pageNumber, int pageSize,
        CancellationToken cancellationToken);
}