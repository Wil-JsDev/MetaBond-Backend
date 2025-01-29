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

        Task<IEnumerable<Events>> FilterByDateRange(DateTime dateFilter,CancellationToken cancellationToken);

        Task<IEnumerable<Events>> GetCommunitiesAndParticipationInEvent(Guid id,CancellationToken cancellationToken);
    }
}
