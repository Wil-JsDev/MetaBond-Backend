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
    Task<PagedResult<Events>> GetPagedEventsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves events that contain the specified title with pagination.
    /// </summary>
    Task<PagedResult<Events>> GetFilterByTitleAsync(string title, int pageNumber, int pageSize,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all events ordered by their ID in ascending order with pagination.
    /// </summary>
    Task<PagedResult<Events>> GetOrderByIdAscAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all events ordered by their ID in descending order with pagination.
    /// </summary>
    Task<PagedResult<Events>> GetOrderByIdDescAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

    /// <summary>
    /// Filters events by a specific date for a given community with pagination.
    /// </summary>
    Task<PagedResult<Events>> FilterByDateRangeAsync(Guid communitiesId, DateTime dateFilter, int pageNumber,
        int pageSize, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves events associated with a specific community with pagination.
    /// </summary>
    Task<PagedResult<Events>> GetCommunitiesAsync(Guid id, int pageNumber, int pageSize,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves an event along with its participations with pagination.
    /// </summary>
    Task<PagedResult<Events>> GetEventsWithParticipationAsync(Guid eventId, int pageNumber, int pageSize,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves events that match a specific title and belong to a given community.
    /// </summary>
    Task<PagedResult<Events>> GetEventsByTitleAndCommunityIdAsync(Guid communitiesId, string title,
        int pageNumber, int pageSize,
        CancellationToken cancellationToken);
}