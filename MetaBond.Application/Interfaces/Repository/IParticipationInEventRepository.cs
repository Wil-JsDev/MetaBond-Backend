using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Interfaces.Repository
{
    public interface IParticipationInEventRepository : IGenericRepository<ParticipationInEvent>
    {
        Task<PagedResult<ParticipationInEvent>> GetPagedParticipationInEventAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<IEnumerable<ParticipationInEvent>> GetEventsAsync(Guid participationInEventId, CancellationToken cancellationToken);
    }
}
