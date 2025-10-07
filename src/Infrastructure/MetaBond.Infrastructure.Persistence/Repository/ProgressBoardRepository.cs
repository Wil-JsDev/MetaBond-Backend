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

        public async Task<PagedResult<ProgressBoard>> GetBoardsByDateRangeAsync(DateTime startTime, DateTime endTime,
            int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var baseQuery = _metaBondContext.Set<ProgressBoard>()
                .AsNoTracking()
                .Where(x => x.CreatedAt <= startTime && x.CreatedAt <= endTime);

            var total = await baseQuery.CountAsync(cancellationToken);

            var query = await baseQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var pagedResponse = new PagedResult<ProgressBoard>(query, pageNumber, pageSize, total);

            return pagedResponse;
        }

        public async Task<PagedResult<ProgressBoard>> GetBoardsWithEntriesAsync(Guid id,
            int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var baseQuery = _metaBondContext.Set<ProgressBoard>()
                .AsNoTracking()
                .Where(x => x.Id == id);

            var total = await baseQuery.CountAsync(cancellationToken);

            var query = await baseQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(x => x.ProgressEntries)
                .AsSplitQuery()
                .ToListAsync(cancellationToken);

            var pagedResponse = new PagedResult<ProgressBoard>(query, pageNumber, pageSize, total);

            return pagedResponse;
        }

        public async Task<PagedResult<ProgressBoard>> GetProgressBoardsWithAuthorAsync(Guid progressBoardId,
            int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var baseQuery = _metaBondContext.Set<ProgressBoard>()
                .AsNoTracking()
                .Where(x => x.Id == progressBoardId);

            var total = await baseQuery.CountAsync(cancellationToken);

            var query = await baseQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(x => x.User)
                .Include(x => x.ProgressEntries)
                .Include(x => x.Communities)
                .AsSplitQuery()
                .ToListAsync(cancellationToken);

            var pagedResponse = new PagedResult<ProgressBoard>(query, pageNumber, pageSize, total);

            return pagedResponse;
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

        public async Task<PagedResult<ProgressBoard>> GetRecentBoardsAsync(DateTime dateTime,
            int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var baseQuery = _metaBondContext.Set<ProgressBoard>()
                .AsNoTracking()
                .Where(x => x.CreatedAt < dateTime);

            var total = await baseQuery.CountAsync(cancellationToken);

            var query = await baseQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var pagedResponse = new PagedResult<ProgressBoard>(query, pageNumber, pageSize, total);

            return pagedResponse;
        }
    }
}