using System.Linq.Expressions;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;
using MetaBond.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MetaBond.Infrastructure.Persistence.Repository;

public class CommunitiesRepository(MetaBondContext metaBondContext)
    : GenericRepository<Communities>(metaBondContext), ICommunitiesRepository
{
    public async Task<PagedResult<Communities>> GetPagedCommunitiesByCategoryIdAsync(int numberPaged, int pageSize,
        Guid categoryId,
        CancellationToken cancellationToken)
    {
        var totalRecord = await _metaBondContext.Set<Communities>()
            .AsNoTracking()
            .Where(cm => cm.CommunityCategoryId == categoryId)
            .CountAsync(cancellationToken);

        var pagedCommunities = await _metaBondContext.Set<Communities>()
            .AsNoTracking()
            .Where(cm => cm.CommunityCategoryId == categoryId)
            .OrderBy(c => c.Id)
            .Skip((numberPaged - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Communities>(pagedCommunities, numberPaged, pageSize, totalRecord);
    }

    public async Task<PagedResult<Communities>> GetPagedCommunitiesAsync(
        int pageNumber,
        int pageZize,
        CancellationToken cancellationToken)
    {
        var totalRecord = await _metaBondContext.Set<Communities>()
            .AsNoTracking()
            .CountAsync(cancellationToken);

        var pagedCommunities = await _metaBondContext.Set<Communities>().AsNoTracking()
            .OrderBy(c => c.Id)
            .Skip((pageNumber - 1) * pageZize)
            .Take(pageZize)
            .ToListAsync(cancellationToken);

        var pagedResponse = new PagedResult<Communities>(pagedCommunities, pageNumber, pageZize, totalRecord);
        return pagedResponse;
    }

    public async Task<PagedResult<Communities>> GetPostsAndEventsByCommunityIdAsync(
        Guid communitieId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var baseQuery = _metaBondContext.Set<Communities>()
            .AsNoTracking()
            .Where(x => x.Id == communitieId);

        var total = await baseQuery.CountAsync(cancellationToken);

        var communities = await baseQuery
            .OrderBy(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Include(x => x.Posts)
            .Include(x => x.Events)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        return new PagedResult<Communities>(communities, pageNumber, pageSize, total);
    }

    public async Task<bool> ValidateAsync(Expression<Func<Communities, bool>> predicate) =>
        await _metaBondContext.Set<Communities>().AnyAsync(predicate);
}