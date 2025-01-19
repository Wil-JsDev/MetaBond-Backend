using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Pagination;
using MetaBond.Domain;
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
    public class EventsRepository : GenericRepository<Events>, IEventsRepository
    {
        public EventsRepository(MetaBondContext metaBondContext) : base(metaBondContext)
        {
        }

        public async Task<PagedResult<Events>> GetPagedEventsAsync(
            int pageNumber, 
            int pageZize, 
            CancellationToken cancellationToken)
        {
            var totalRecord = await _metaBondContext.Set<Events>()
                .AsNoTracking()
                .CountAsync(cancellationToken);
            
            var pagedEvents = await _metaBondContext.Set<Events>()
                                                    .AsNoTracking()
                                                    .OrderBy(e => e.Id)
                                                    .Skip((pageNumber - 1) * pageZize)
                                                    .Take(pageZize)
                                                    .ToListAsync(cancellationToken);

            PagedResult<Events> result = new(pagedEvents,pageNumber,pageZize,totalRecord);
            return result;
        }
        public async Task<IEnumerable<Events>> GetFilterByTitleAsync(string title, CancellationToken cancellationToken)
        {
            var query = await _metaBondContext.Set<Events>()
                                              .AsNoTracking()
                                              .Where(e => e.Title == title)
                                              .ToListAsync(cancellationToken);
            return query;
        }

        public async Task<IEnumerable<Events>> GetOrderByIdAscAsync(Guid orderId, CancellationToken cancellationToken)
        {
            var query = await _metaBondContext.Set<Events>()
                                         .AsNoTracking()
                                         .OrderBy(x => x.Id == orderId)
                                         .ToListAsync(cancellationToken);
            return query;
        }

        public async Task<IEnumerable<Events>> GetOrderByIdDescAsync(Guid orderId, CancellationToken cancellationToken)
        {
            var query = await _metaBondContext.Set<Events>()
                                         .AsNoTracking()
                                         .OrderByDescending(x => x.Id == orderId)
                                         .ToListAsync(cancellationToken);
            return query;
        }

        public async Task<IEnumerable<Events>> FilterByDateRange(DateTime dateFilter, CancellationToken cancellationToken)
        {
            var query = await _metaBondContext.Set<Events>()
                                              .AsNoTracking()
                                              .Where(x => x.CreateAt == dateFilter)
                                              .ToListAsync(cancellationToken);

            return query;
        }
    }
}
