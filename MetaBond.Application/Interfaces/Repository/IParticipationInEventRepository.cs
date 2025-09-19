using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Interfaces.Repository;

/// <summary>
/// Repository interface for managing participation records in events.
/// Inherits from the generic repository interface.
/// </summary>
public interface IParticipationInEventRepository : IGenericRepository<ParticipationInEvent>
{
    /// <summary>
    /// Retrieves a paginated list of event participations.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of records per page.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A paginated result of event participations.</returns>
    Task<PagedResult<ParticipationInEvent>> GetPagedParticipationInEventAsync(int pageNumber, int pageSize,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the event(s) associated with a specific participation record.
    /// </summary>
    /// <param name="participationInEventId">The ID of the participation record.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A collection of events linked to the specified participation record.</returns>
    Task<IEnumerable<ParticipationInEvent>> GetEventsAsync(Guid participationInEventId,
        CancellationToken cancellationToken);
}