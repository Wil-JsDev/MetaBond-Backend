using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;
using MetaBond.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MetaBond.Infrastructure.Persistence.Repository
{
    public class RewardsRepository : GenericRepository<Rewards>, IRewardsRepository
    {
        public RewardsRepository(MetaBondContext metaBondContext) : base(metaBondContext)
        {
        }

        public async Task<PagedResult<Rewards>> GetPagedRewardsAsync(int pageNumber, int pageZize, CancellationToken cancellationToken)
        {
            var totalRecord = await _metaBondContext.Set<Rewards>().AsNoTracking().CountAsync(cancellationToken);

            var rewards = await _metaBondContext.Set<Rewards>().AsNoTracking()
                        .OrderBy(r => r.Id)
                        .Skip((pageNumber - 1) * pageZize)
                        .Take(pageZize)
                        .ToListAsync(cancellationToken);

            var pagedResponse = new PagedResult<Rewards>(rewards,pageNumber,pageZize,totalRecord);
            return pagedResponse;
        }
    }
}
