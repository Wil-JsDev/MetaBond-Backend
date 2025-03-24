using MetaBond.Application.Pagination;
using MetaBond.Domain;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Interfaces.Repository
{
    public interface IEventsRepository : IGenericRepository<Events>
    {
        Task<PagedResult<Events>> GetPagedEventsAsync(int pageNumber, int pageZize, CancellationToken cancellationToken);

        Task<IEnumerable<Events>> GetFilterByTitleAsync(string title, CancellationToken cancellationToken);

        Task<IEnumerable<Events>> GetOrderByIdAscAsync(CancellationToken cancellationToken);

        Task<IEnumerable<Events>> GetOrderByIdDescAsync(CancellationToken cancellationToken);

        Task<IEnumerable<Events>> FilterByDateRange(Guid communitiesId,DateTime dateFilter,CancellationToken cancellationToken);

        Task<IEnumerable<Events>> GetCommunities(Guid id,CancellationToken cancellationToken);

        Task<IEnumerable<Events>> GetEventsWithParticipationAsync(Guid eventId, CancellationToken cancellationToken);

        Task<IEnumerable<Events>> GetEventsByTitleAndCommunityIdAsync(Guid communitiesId, string title, CancellationToken cancellationToken);
    }
}
