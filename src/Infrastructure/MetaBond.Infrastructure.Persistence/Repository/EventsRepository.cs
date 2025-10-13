using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;
using MetaBond.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MetaBond.Infrastructure.Persistence.Repository;

public class EventsRepository(MetaBondContext metaBondContext)
    : GenericRepository<Events>(metaBondContext), IEventsRepository
{
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

        PagedResult<Events> result = new(pagedEvents, pageNumber, pageZize, totalRecord);
        return result;
    }

    public async Task<PagedResult<Events>> GetFilterByTitleAsync(string title, int pageNumber, int pageSize,
        CancellationToken cancellationToken)
    {
        var baseQuery = _metaBondContext.Set<Events>()
            .AsNoTracking()
            .Where(e => e.Title == title);

        var total = await baseQuery.CountAsync(cancellationToken);

        var query = await baseQuery
            .OrderBy(e => e.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var pagedResponse = new PagedResult<Events>(query, pageNumber, pageSize, total);

        return pagedResponse;
    }

    public async Task<PagedResult<Events>> GetEventsByTitleAndCommunityIdAsync(Guid communitiesId, string title,
        int pageNumber, int pageSize,
        CancellationToken cancellationToken)
    {
        var baseQuery = _metaBondContext.Set<Events>()
            .AsNoTracking()
            .Where(e => e.CommunitiesId == communitiesId && e.Title == title);

        var total = await baseQuery.CountAsync(cancellationToken);

        var query = await baseQuery
            .OrderBy(e => e.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var pagedResponse = new PagedResult<Events>(query, pageNumber, pageSize, total);

        return pagedResponse;
    }

    public async Task<PagedResult<Events>> GetOrderByIdAscAsync(int pageNumber, int pageSize,
        CancellationToken cancellationToken)
    {
        var totalRecord = await _metaBondContext.Set<Events>()
            .AsNoTracking()
            .CountAsync(cancellationToken);

        var pagedEvents = await _metaBondContext.Set<Events>()
            .AsNoTracking()
            .OrderBy(e => e.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        PagedResult<Events> result = new(pagedEvents, pageNumber, pageSize, totalRecord);

        return result;
    }

    public async Task<PagedResult<Events>> GetOrderByIdDescAsync(int pageNumber, int pageSize,
        CancellationToken cancellationToken)
    {
        var total = await _metaBondContext.Set<Events>()
            .AsNoTracking()
            .CountAsync(cancellationToken);

        var query = await _metaBondContext.Set<Events>()
            .AsNoTracking()
            .OrderByDescending(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var pagedResponse = new PagedResult<Events>(query, pageNumber, pageSize, total);

        return pagedResponse;
    }

    public async Task<PagedResult<Events>> FilterByDateRangeAsync(Guid communitiesId, DateTime dateFilter,
        int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var baseQuery = _metaBondContext.Set<Events>()
            .AsNoTracking()
            .Where(x => x.CommunitiesId == communitiesId && x.CreateAt == dateFilter);

        var total = await baseQuery.CountAsync(cancellationToken);

        var query = await baseQuery
            .OrderBy(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var pagedResult = new PagedResult<Events>(query, pageNumber, pageSize, total);
        return pagedResult;
    }

    public async Task<PagedResult<Events>> GetCommunitiesAsync(Guid id, int pageNumber, int pageSize,
        CancellationToken cancellationToken)
    {
        var baseQuery = _metaBondContext.Set<Events>()
            .AsNoTracking()
            .Where(x => x.Id == id);

        var total = await baseQuery.CountAsync(cancellationToken);

        var query = await baseQuery
            .OrderBy(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Include(x => x.Communities)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        PagedResult<Events> result = new(query, pageNumber, pageSize, total);

        return result;
    }

    public async Task<PagedResult<Events>> GetEventsWithParticipationAsync(Guid eventId,
        int pageNumber, int pageSize,
        CancellationToken cancellationToken)
    {
        var baseQuery = _metaBondContext.Set<Events>()
            .AsNoTracking()
            .Where(x => x.Id == eventId);

        var total = await baseQuery.CountAsync(cancellationToken);

        var query = await baseQuery
            .OrderBy(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Include(x => x.EventParticipations)!
            .ThenInclude(x => x.ParticipationInEvent)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        PagedResult<Events> result = new(query, pageNumber, pageSize, total);

        return result;
    }
}