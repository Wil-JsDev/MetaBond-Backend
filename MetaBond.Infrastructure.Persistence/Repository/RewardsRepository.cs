using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;
using MetaBond.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MetaBond.Infrastructure.Persistence.Repository
{
    public class RewardsRepository(MetaBondContext metaBondContext)
        : GenericRepository<Rewards>(metaBondContext), IRewardsRepository
    {
        public async Task<PagedResult<Rewards>> GetPagedRewardsAsync(
            int pageNumber,
            int pageZize,
            CancellationToken cancellationToken)
        {
            var totalRecord = await _metaBondContext.Set<Rewards>().AsNoTracking().CountAsync(cancellationToken);

            var rewards = await _metaBondContext.Set<Rewards>().AsNoTracking()
                .OrderBy(r => r.Id)
                .Skip((pageNumber - 1) * pageZize)
                .Take(pageZize)
                .ToListAsync(cancellationToken);

            var pagedResponse = new PagedResult<Rewards>(rewards, pageNumber, pageZize, totalRecord);
            return pagedResponse;
        }

        public async Task<Rewards> GetMostRecentRewardAsync(CancellationToken cancellationToken)
        {
            var reward = await _metaBondContext.Set<Rewards>()
                .AsNoTracking()
                .OrderByDescending(r => r.DateAwarded)
                .FirstOrDefaultAsync(cancellationToken);

            return reward;
        }

        public async Task<IEnumerable<Rewards>> GetRewardsByDateRangeAsync(
            DateTime startTime,
            DateTime endTime,
            CancellationToken cancellationToken)
        {
            var query = await _metaBondContext.Set<Rewards>()
                .AsNoTracking()
                .Where(x => x.DateAwarded >= startTime && x.DateAwarded <= endTime)
                .ToListAsync(cancellationToken);
            return query;
        }

        public async Task<int> CountRewardsAsync(CancellationToken cancellationToken)
        {
            int query = await _metaBondContext.Set<Rewards>()
                .AsNoTracking()
                .CountAsync(cancellationToken);

            return query;
        }

        public async Task<IEnumerable<Rewards>> GetTopRewardsByPointsAsync(
            int topCount,
            CancellationToken cancellationToken)
        {
            var query = await _metaBondContext.Set<Rewards>()
                .AsNoTracking()
                .OrderByDescending(x => x.PointAwarded)
                .Include(r => r.User)
                .AsSplitQuery()
                .Take(topCount)
                .ToListAsync(cancellationToken);

            return query;
        }

        public async Task<IEnumerable<Rewards>> GetUsersByRewardIdAsync(Guid rewardId,
            CancellationToken cancellationToken)
        {
            return await _metaBondContext.Set<Rewards>()
                .AsNoTracking()
                .Where(x => x.Id == rewardId)
                .Include(x => x.User)
                .AsSplitQuery()
                .ToListAsync(cancellationToken);
        }
    }
}