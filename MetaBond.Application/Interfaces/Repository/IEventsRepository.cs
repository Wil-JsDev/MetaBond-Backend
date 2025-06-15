using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Interfaces.Repository;

/// <summary>
/// Repository interface for accessing and managing event entities.
/// Inherits from the generic repository interface.
/// </summary>
public interface IEventsRepository : IGenericRepository<Events>
{
    /// <summary>
    /// Retrieves a paginated list of events.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of events per page.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A paginated result of events.</returns>
    Task<PagedResult<Events>> GetPagedEventsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves events that contain the specified title.
    /// </summary>
    /// <param name="title">The title or partial title to filter events.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A collection of events matching the title.</returns>
    Task<IEnumerable<Events>> GetFilterByTitleAsync(string title, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all events ordered by their ID in ascending order.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A collection of events ordered by ID ascending.</returns>
    Task<IEnumerable<Events>> GetOrderByIdAscAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all events ordered by their ID in descending order.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A collection of events ordered by ID descending.</returns>
    Task<IEnumerable<Events>> GetOrderByIdDescAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Filters events within a specific date range for a given community.
    /// </summary>
    /// <param name="communitiesId">The ID of the community.</param>
    /// <param name="dateFilter">The date to use as a filter (e.g., start or specific day).</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A collection of events within the date range for the community.</returns>
    Task<IEnumerable<Events>> FilterByDateRange(Guid communitiesId, DateTime dateFilter, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves events related to a specific community.
    /// </summary>
    /// <param name="id">The ID of the community.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A collection of events associated with the given community.</returns>
    Task<IEnumerable<Events>> GetCommunities(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves an event along with its participations.
    /// </summary>
    /// <param name="eventId">The ID of the event.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A collection of events including participation data.</returns>
    Task<IEnumerable<Events>> GetEventsWithParticipationAsync(Guid eventId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves events that match a specific title and belong to a given community.
    /// </summary>
    /// <param name="communitiesId">The ID of the community.</param>
    /// <param name="title">The title or partial title to filter events.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A collection of events that match the title and community ID.</returns>
    Task<IEnumerable<Events>> GetEventsByTitleAndCommunityIdAsync(Guid communitiesId, string title, CancellationToken cancellationToken);
}