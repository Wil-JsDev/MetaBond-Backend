﻿using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;
using MetaBond.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

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
        
        public async Task<IEnumerable<Events>> GetEventsByTitleAndCommunityIdAsync(Guid communitiesId, string title, CancellationToken cancellationToken)
        {
            var query = await _metaBondContext.Set<Events>()
                .AsNoTracking()
                .Where(e => e.CommunitiesId == communitiesId && e.Title == title)
                .ToListAsync(cancellationToken);
            return query;
        }
        
        public async Task<IEnumerable<Events>> GetOrderByIdAscAsync(CancellationToken cancellationToken)
        {
            var query = await _metaBondContext.Set<Events>()
                                         .AsNoTracking()
                                         .OrderBy(x => x.Id)
                                         .ToListAsync(cancellationToken);
            return query;
        }

        public async Task<IEnumerable<Events>> GetOrderByIdDescAsync(CancellationToken cancellationToken)
        {
            var query = await _metaBondContext.Set<Events>()
                                         .AsNoTracking()
                                         .OrderByDescending(x => x.Id)
                                         .ToListAsync(cancellationToken);
            return query;
        }

        public async Task<IEnumerable<Events>> FilterByDateRange(Guid communitiesId,DateTime dateFilter, CancellationToken cancellationToken)
        {
            var query = await _metaBondContext.Set<Events>()
                                              .AsNoTracking()
                                              .Where(x => x.CommunitiesId == communitiesId &&  x.CreateAt == dateFilter)
                                              .ToListAsync(cancellationToken);

            return query;
        }

        public async Task<IEnumerable<Events>> GetCommunities(Guid id, CancellationToken cancellationToken)
        {
            var query = await _metaBondContext.Set<Events>()
                                               .AsNoTracking()
                                               .Where(x => x.Id == id) 
                                               .Include(x => x.Communities)
                                               .AsSplitQuery()
                                               .ToListAsync(cancellationToken);
            return query;
        }

        public async Task<IEnumerable<Events>> GetEventsWithParticipationAsync(Guid eventId, CancellationToken cancellationToken)
        {
            var query = await _metaBondContext.Set<Events>()
                                               .Where(x => x.Id == eventId)     
                                               .Include(ep => ep.EventParticipations)
                                                    .ThenInclude(ev => ev.ParticipationInEvent)
                                               .AsSplitQuery()
                                               .ToListAsync(cancellationToken);

            return query;
        }

    }
}
