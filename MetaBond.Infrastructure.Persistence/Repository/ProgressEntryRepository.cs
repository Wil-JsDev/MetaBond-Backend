using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;
using MetaBond.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Infrastructure.Persistence.Repository
{
    public class ProgressEntryRepository : GenericRepository<ProgressEntry>, IProgressEntryRepository
    {
        public ProgressEntryRepository(MetaBondContext metaBondContext) : base(metaBondContext)
        {
        }

        public async Task<int> CountEntriesByBoardIdAsync(
            Guid ProgressBoardId, 
            CancellationToken cancellationToken)
        {
            var query = await _metaBondContext.Set<ProgressEntry>()
                                              .AsNoTracking()
                                              .CountAsync(x => x.ProgressBoardId == ProgressBoardId, cancellationToken);
            return query;
        }

        public async Task<IEnumerable<ProgressEntry>> GetEntriesByDateRangeAsync(
            DateTime startTime, 
            DateTime endTime, 
            CancellationToken cancellationToken)
        {
            var query = await _metaBondContext.Set<ProgressEntry>()
                                               .AsNoTracking()
                                               .Where(x => x.CreatedAt >= startTime && x.CreatedAt <= endTime)
                                               .ToListAsync(cancellationToken);

            return query;
        }

        public async Task<IEnumerable<ProgressEntry>> GetOrderByDescriptionAsync(CancellationToken cancellationToken)
        {
            var query = await _metaBondContext.Set<ProgressEntry>()
                                               .AsNoTracking()
                                               .OrderBy(x => x.Description)
                                               .ToListAsync(cancellationToken);

            return query;
        }

        public async Task<IEnumerable<ProgressEntry>> GetRecentEntriesAsync(
            int topCount, 
            CancellationToken cancellationToken)
        {
            var query = await _metaBondContext.Set<ProgressEntry>()
                                               .AsNoTracking()
                                               .OrderByDescending(x => x.CreatedAt)
                                               .Take(topCount)
                                               .ToListAsync(cancellationToken);

            return query;
        }

        public async Task<IEnumerable<ProgressEntry>> GetOrderByIdAsync(CancellationToken cancellationToken)
        {

            var query = await _metaBondContext.Set<ProgressEntry>()
                                               .AsNoTracking()
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

            var pagedResponse = new PagedResult<ProgressEntry>(getPagedProgressEntry,pageNumber,pageSize,totalRecord);
            return pagedResponse;
        }

    }
}
