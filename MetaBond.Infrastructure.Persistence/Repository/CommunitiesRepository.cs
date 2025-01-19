using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;
using MetaBond.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MetaBond.Infrastructure.Persistence.Repository
{
    public class CommunitiesRepository : GenericRepository<Communities>, ICommunitiesRepository
    {
        public CommunitiesRepository(MetaBondContext metaBondContext) : base(metaBondContext)
        {

        }

        public async Task<IEnumerable<Communities>> GetByFilterAsync(
            Func<Communities, bool> predicate,
            CancellationToken cancellationToken)
        {
            var query = await Task.Run(() => _metaBondContext.Set<Communities>()
                                              .AsNoTracking()
                                              .Where(predicate)
                                              .ToList(), cancellationToken);
                                             
            return query;
        }       

        public async Task<PagedResult<Communities>> GetPagedCommunitiesAsync(
            int pageNumber, 
            int pageZize, 
            CancellationToken cancellationToken)
        {

            var totalRecord = await _metaBondContext.Set<Communities>()
                                                    .AsNoTracking()
                                                    .CountAsync();

            var pagedCommunities = await _metaBondContext.Set<Communities>().AsNoTracking()
                                                         .OrderBy(c => c.Id)
                                                         .Skip((pageNumber - 1 ) * pageZize)
                                                         .Take(pageZize)
                                                         .ToListAsync(cancellationToken);

            var pagedResponse = new PagedResult<Communities>(pagedCommunities,pageNumber,pageZize,totalRecord);
            return pagedResponse;

        }

        
    }
}
