using System.Linq.Expressions;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;
using MetaBond.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MetaBond.Infrastructure.Persistence.Repository;

public class CommunityManagerRepositoryRepository(MetaBondContext metaBondContext)
    : GenericRepository<CommunityManager>(metaBondContext), ICommunityManagerRepository
{
    public async Task CreateCommunityManagerAsync(CommunityManager communityManager, CancellationToken cancellationToken)
    {
        await _metaBondContext.Set<CommunityManager>().AddAsync(communityManager, cancellationToken);
    }

    public async Task<PagedResult<Domain.Models.CommunityManager>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        var totalRecord = await _metaBondContext.Set<CommunityManager>()
            .AsNoTracking()
            .CountAsync(cancellationToken);

        var pagedCommunityManager = await _metaBondContext.Set<CommunityManager>()
            .AsNoTracking()
            .OrderBy(cm => cm.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        
        var pagedResult = new PagedResult<CommunityManager>(pagedCommunityManager, page, pageSize, totalRecord);
        
        return pagedResult;
    }

    public async Task<bool> ExistsAsync(Expression<Func<CommunityManager,bool>> predicate, CancellationToken cancellationToken)
    {
        return await _metaBondContext.Set<CommunityManager>()
            .AsNoTracking()
            .AnyAsync(predicate, cancellationToken);
    }

    public async Task<bool> IsUserCommunityManagerAsync(Guid userId, Guid communityId, CancellationToken cancellationToken)
    {
        return await _metaBondContext.Set<CommunityManager>()
            .AsNoTracking()
            .AnyAsync(cm => cm.CommunityId == communityId && cm.UserId == userId, cancellationToken);
    }

    public async Task<List<CommunityManager>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _metaBondContext.Set<CommunityManager>()
            .AsNoTracking()
            .Where(cm => cm.UserId == userId)
            .Include(cm => cm.User)
            .Include(cm => cm.Community)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<CommunityManager>> GetByCommunityIdAsync(Guid communityId, CancellationToken cancellationToken)
    {
        return await _metaBondContext.Set<CommunityManager>()
            .AsNoTracking()
            .Where(cm => cm.CommunityId == communityId)
            .Include(cm => cm.User)
            .Include(cm => cm.Community)
            .ToListAsync(cancellationToken);
    }
}