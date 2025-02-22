using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;
using MetaBond.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MetaBond.Infrastructure.Persistence.Repository
{
    public class ParticipationInEventRepository : GenericRepository<ParticipationInEvent>, IParticipationInEventRepository
    {
        public ParticipationInEventRepository(MetaBondContext metaBondContext) : base(metaBondContext)
        {
        }

        public async Task<PagedResult<ParticipationInEvent>> GetPagedParticipationInEventAsync(
            int pageNumber, 
            int pageSize, 
            CancellationToken cancellationToken)
        {

            var totalRecord = await _metaBondContext.Set<ParticipationInEvent>().AsNoTracking().CountAsync(cancellationToken);

            var getPagedParticipationInEvent = await _metaBondContext.Set<ParticipationInEvent>()
                                                                     .AsNoTracking()
                                                                     .OrderBy(x => x.Id)
                                                                     .Skip((pageNumber - 1) * pageSize)
                                                                     .Take(pageSize)
                                                                     .ToListAsync(cancellationToken);
            
            PagedResult<ParticipationInEvent> pageResponse = new PagedResult<ParticipationInEvent>(getPagedParticipationInEvent, pageNumber, pageSize, totalRecord);
            return pageResponse;
        }

        public async Task<IEnumerable<ParticipationInEvent>> GetParticipationByEventIdAsync(Guid idEvent, CancellationToken cancellationToken)
        {
            var query = await _metaBondContext.Set<ParticipationInEvent>()
                                               .AsNoTracking()
                                               .Where(x => x.EventId == idEvent)
                                               .AsSplitQuery()
                                               .ToListAsync(cancellationToken);

            return query;
        }
    }
}
