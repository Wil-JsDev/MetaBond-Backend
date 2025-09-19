using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;
using MetaBond.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;


namespace MetaBond.Infrastructure.Persistence.Repository
{
    public class ProgressBoardRepository(MetaBondContext metaBondContext)
        : GenericRepository<ProgressBoard>(metaBondContext), IProgressBoardRepository
    {
        public async Task<int> CountBoardsAsync(CancellationToken cancellationToken)
        {
            var query = await _metaBondContext.Set<ProgressBoard>()
                .AsNoTracking()
                .CountAsync(cancellationToken);

            return query;
        }

        public async Task<IEnumerable<ProgressBoard>> GetBoardsByDateRangeAsync(DateTime startTime, DateTime endTime,
            CancellationToken cancellationToken)
        {
            var query = await _metaBondContext.Set<ProgressBoard>()
                .AsNoTracking()
                .Where(x => x.CreatedAt <= startTime && x.CreatedAt <= endTime)
                .ToListAsync(cancellationToken);

            return query;
        }

        public async Task<IEnumerable<ProgressBoard>> GetBoardsWithEntriesAsync(Guid id,
            CancellationToken cancellationToken)
        {
            var query = await _metaBondContext.Set<ProgressBoard>()
                .Where(x => x.Id == id)
                .Include(x => x.ProgressEntries)
                .AsSplitQuery()
                .ToListAsync(cancellationToken);

            return query;
        }

        public async Task<IEnumerable<ProgressBoard>> GetProgressBoardsWithAuthorAsync(Guid progressBoardId,
            CancellationToken cancellationToken)
        {
            return await _metaBondContext.Set<ProgressBoard>()
                .AsNoTracking()
                .Where(x => x.Id == progressBoardId)
                .Include(x => x.User)
                .Include(x => x.ProgressEntries)
                .Include(x => x.Communities)
                .AsSplitQuery()
                .ToListAsync(cancellationToken);
        }

        public async Task<PagedResult<ProgressBoard>> GetPagedBoardsAsync(int pageNumber, int pageSize,
            CancellationToken cancellationToken)
        {
            var totalRecord = await _metaBondContext.Set<ProgressBoard>()
                .AsNoTracking()
                .CountAsync(cancellationToken);

            var getPagedProgressBoard = await _metaBondContext.Set<ProgressBoard>()
                .AsNoTracking()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var pagedResponse =
                new PagedResult<ProgressBoard>(getPagedProgressBoard, pageNumber, pageSize, totalRecord);
            return pagedResponse;
        }

        public async Task<IEnumerable<ProgressBoard>> GetRecentBoardsAsync(DateTime dateTime,
            CancellationToken cancellationToken)
        {
            var query = Task.Run(() => _metaBondContext.Set<ProgressBoard>()
                .AsNoTracking()
                .Where(x => x.CreatedAt < dateTime)
                .ToList(), cancellationToken);

            return await query;
        }
    }
}