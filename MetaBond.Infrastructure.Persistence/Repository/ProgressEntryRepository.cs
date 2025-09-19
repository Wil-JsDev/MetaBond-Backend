using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;
using MetaBond.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MetaBond.Infrastructure.Persistence.Repository
{
    public class ProgressEntryRepository(MetaBondContext metaBondContext)
        : GenericRepository<ProgressEntry>(metaBondContext), IProgressEntryRepository
    {
        public async Task<int> CountEntriesByBoardIdAsync(
            Guid progressBoardId,
            CancellationToken cancellationToken)
        {
            var query = await _metaBondContext.Set<ProgressEntry>()
                .AsNoTracking()
                .CountAsync(x => x.ProgressBoardId == progressBoardId, cancellationToken);
            return query;
        }

        public async Task<IEnumerable<ProgressEntry>> GetByIdProgressEntryWithProgressBoard(Guid progressEntry,
            CancellationToken cancellationToken)
        {
            var query = await _metaBondContext.Set<ProgressEntry>()
                .AsNoTracking()
                .Where(x => x.Id == progressEntry)
                .Include(x => x.ProgressBoard)
                .AsSplitQuery()
                .ToListAsync(cancellationToken);

            return query;
        }

        public async Task<IEnumerable<ProgressEntry>> GetProgressEntriesWithAuthorsAsync(Guid progressEntryId,
            CancellationToken cancellationToken)
        {
            return await _metaBondContext.Set<ProgressEntry>()
                .AsNoTracking()
                .Where(x => x.Id == progressEntryId)
                .Include(x => x.User)
                .AsSplitQuery()
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ProgressEntry>> GetEntriesByDateRangeAsync(
            Guid progressBoardId,
            DateTime startTime,
            DateTime endTime,
            CancellationToken cancellationToken)
        {
            var query = await _metaBondContext.Set<ProgressEntry>()
                .AsNoTracking()
                .Where(x => x.ProgressBoardId == progressBoardId)
                .Where(x => x.CreatedAt >= startTime && x.CreatedAt <= endTime)
                .ToListAsync(cancellationToken);

            return query;
        }

        public async Task<IEnumerable<ProgressEntry>> GetOrderByDescriptionAsync(Guid progressBoard,
            CancellationToken cancellationToken)
        {
            var query = await _metaBondContext.Set<ProgressEntry>()
                .AsNoTracking()
                .Where(x => x.ProgressBoardId == progressBoard)
                .OrderBy(x => x.Description)
                .ToListAsync(cancellationToken);

            return query;
        }

        public async Task<IEnumerable<ProgressEntry>> GetRecentEntriesAsync(
            Guid progressBoard,
            int topCount,
            CancellationToken cancellationToken)
        {
            var query = await _metaBondContext.Set<ProgressEntry>()
                .AsNoTracking()
                .Where(x => x.ProgressBoardId == progressBoard)
                .OrderByDescending(x => x.CreatedAt)
                .Take(topCount)
                .ToListAsync(cancellationToken);

            return query;
        }

        public async Task<IEnumerable<ProgressEntry>> GetOrderByIdAsync(Guid progressBoardId,
            CancellationToken cancellationToken)
        {
            var query = await _metaBondContext.Set<ProgressEntry>()
                .AsNoTracking()
                .Where(x => x.ProgressBoardId == progressBoardId)
                .OrderBy(x => x.Id)
                .ToListAsync(cancellationToken);


            return query;
        }

        public async Task<PagedResult<ProgressEntry>> GetPagedProgressEntryAsync(
            int pageSize,
            int pageNumber,
            CancellationToken cancellationToken)
        {
            var totalRecord = await _metaBondContext.Set<ProgressEntry>()
                .AsNoTracking()
                .CountAsync(cancellationToken);

            var getPagedProgressEntry = await _metaBondContext.Set<ProgressEntry>()
                .AsNoTracking()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var pagedResponse =
                new PagedResult<ProgressEntry>(getPagedProgressEntry, pageNumber, pageSize, totalRecord);
            return pagedResponse;
        }
    }
}