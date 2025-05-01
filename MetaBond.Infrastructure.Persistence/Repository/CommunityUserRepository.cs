using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;
using MetaBond.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MetaBond.Infrastructure.Persistence.Repository;

public class CommunityUserRepository(MetaBondContext metaBondContext)
    : GenericRepository<CommunityUser>(metaBondContext), ICommunityUserRepository
{
    public async Task<PagedResult<CommunityUser>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var totalRecord = await _metaBondContext.Set<CommunityUser>().CountAsync(cancellationToken);
        
        var paged = await _metaBondContext.Set<CommunityUser>()
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<CommunityUser>(paged, pageNumber,pageSize, totalRecord);
    }

    public async Task<IEnumerable<CommunityUser>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _metaBondContext.Set<CommunityUser>()
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Include(us => us.Community)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<CommunityUser>> GetByCommunityIdAsync(Guid communityId, CancellationToken cancellationToken)
    {
        return await _metaBondContext.Set<CommunityUser>()
            .AsNoTracking()
            .Where(x => x.CommunityId == communityId)
            .Include(us => us.User)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<CommunityUser>> GetCommunitiesByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _metaBondContext.Set<CommunityUser>()
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Include(x => x.Community)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid userId, Guid communityId, CancellationToken cancellationToken)
    {
        return await _metaBondContext.Set<CommunityUser>()
            .AsNoTracking()
            .AnyAsync(x => x.UserId == userId && x.CommunityId == communityId, cancellationToken);
    }

    public async Task<int> CountMembersByCommunityIdAsync(Guid communityId, CancellationToken cancellationToken)
    {
        return await  _metaBondContext.Set<CommunityUser>()
            .AsNoTracking()
            .CountAsync(x => x.CommunityId == communityId, cancellationToken);
    }

    public async Task RemoveAsync(Guid userId, Guid communityId, CancellationToken cancellationToken)
    {
        var entity = await _metaBondContext.Set<CommunityUser>()
            .FirstOrDefaultAsync(x => x.UserId == userId && x.CommunityId == communityId, cancellationToken);

        if (entity != null)
        {
            _metaBondContext.Set<CommunityUser>().Remove(entity);
            await SaveAsync(cancellationToken);
        }
        
    }
}