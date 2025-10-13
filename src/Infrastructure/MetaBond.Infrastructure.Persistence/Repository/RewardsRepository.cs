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

        public async Task<PagedResult<Rewards>> GetRewardsByDateRangeAsync(
            DateTime startTime,
            DateTime endTime,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var baseQuery = _metaBondContext.Set<Rewards>()
                .AsNoTracking()
                .Where(x => x.DateAwarded >= startTime && x.DateAwarded <= endTime);

            var total = await baseQuery.CountAsync(cancellationToken);

            var query = await baseQuery
                .OrderBy(x => x.DateAwarded)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var pagedResponse = new PagedResult<Rewards>(query, pageNumber, pageSize, total);
            return pagedResponse;
        }

        public async Task<int> CountRewardsAsync(CancellationToken cancellationToken)
        {
            int query = await _metaBondContext.Set<Rewards>()
                .AsNoTracking()
                .CountAsync(cancellationToken);

            return query;
        }

        public async Task<PagedResult<Rewards>> GetTopRewardsByPointsAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var baseQuery = _metaBondContext.Set<Rewards>()
                .AsNoTracking()
                .OrderByDescending(x => x.PointAwarded);

            var total = await baseQuery.CountAsync(cancellationToken);

            var query = await baseQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(r => r.User)
                .AsSplitQuery()
                .ToListAsync(cancellationToken);

            PagedResult<Rewards> pagedResponse = new(query, pageNumber, pageSize, total);
            return pagedResponse;
        }

        public async Task<PagedResult<Rewards>> GetUsersByRewardIdAsync(Guid rewardId,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var baseQuery = _metaBondContext.Set<Rewards>()
                .AsNoTracking()
                .Where(x => x.Id == rewardId);

            var total = await baseQuery.CountAsync(cancellationToken);

            var query = await baseQuery
                .Include(r => r.User)
                .AsSplitQuery()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            PagedResult<Rewards> pagedResponse = new(query, pageNumber, pageSize, total);
            return pagedResponse;
        }
    }
}