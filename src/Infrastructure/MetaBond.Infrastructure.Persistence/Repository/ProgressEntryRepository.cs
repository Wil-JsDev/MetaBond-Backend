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

        public async Task<PagedResult<ProgressEntry>> GetByIdProgressEntryWithProgressBoard(Guid progressEntry,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var baseQuery = _metaBondContext.Set<ProgressEntry>()
                .AsNoTracking()
                .Where(x => x.Id == progressEntry);

            var total = await baseQuery.CountAsync(cancellationToken);

            var query = await baseQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(x => x.ProgressBoard)
                .AsSplitQuery()
                .ToListAsync(cancellationToken);

            var pagedResponse = new PagedResult<ProgressEntry>(query, pageNumber, pageSize, total);

            return pagedResponse;
        }

        public async Task<PagedResult<ProgressEntry>> GetProgressEntriesWithAuthorsAsync(Guid progressEntryId,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var baseQuery = _metaBondContext.Set<ProgressEntry>()
                .AsNoTracking()
                .Where(x => x.Id == progressEntryId);

            var total = await baseQuery.CountAsync(cancellationToken);

            var query = await baseQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(x => x.User)
                .AsSplitQuery()
                .ToListAsync(cancellationToken);

            var pagedResponse = new PagedResult<ProgressEntry>(query, pageNumber, pageSize, total);

            return pagedResponse;
        }

        public async Task<PagedResult<ProgressEntry>> GetEntriesByDateRangeAsync(
            Guid progressBoardId,
            DateTime startTime,
            DateTime endTime,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var baseQuery = _metaBondContext.Set<ProgressEntry>()
                .AsNoTracking()
                .Where(x => x.ProgressBoardId == progressBoardId)
                .Where(x => x.CreatedAt >= startTime && x.CreatedAt <= endTime);

            var total = await baseQuery.CountAsync(cancellationToken);

            var query = await baseQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var pagedResponse = new PagedResult<ProgressEntry>(query, pageNumber, pageSize, total);

            return pagedResponse;
        }

        public async Task<PagedResult<ProgressEntry>> GetOrderByDescriptionAsync(Guid progressBoard,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var baseQuery = _metaBondContext.Set<ProgressEntry>()
                .AsNoTracking()
                .Where(x => x.ProgressBoardId == progressBoard);

            var total = await baseQuery.CountAsync(cancellationToken);

            var query = await baseQuery
                .OrderBy(x => x.Description)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var pagedResponse = new PagedResult<ProgressEntry>(query, pageNumber, pageSize, total);

            return pagedResponse;
        }

        public async Task<PagedResult<ProgressEntry>> GetRecentEntriesAsync(
            Guid progressBoard,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var baseQuery = _metaBondContext.Set<ProgressEntry>()
                .AsNoTracking()
                .Where(x => x.ProgressBoardId == progressBoard);

            var total = await baseQuery.CountAsync(cancellationToken);

            var query = await baseQuery
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var pagedResponse = new PagedResult<ProgressEntry>(query, pageNumber, pageSize, total);

            return pagedResponse;
        }

        public async Task<PagedResult<ProgressEntry>> GetOrderByIdAsync(Guid progressBoardId,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var baseQuery = _metaBondContext.Set<ProgressEntry>()
                .AsNoTracking()
                .Where(x => x.ProgressBoardId == progressBoardId);

            var total = await baseQuery.CountAsync(cancellationToken);

            var query = await baseQuery
                .OrderBy(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var pagedResponse = new PagedResult<ProgressEntry>(query, pageNumber, pageSize, total);

            return pagedResponse;
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