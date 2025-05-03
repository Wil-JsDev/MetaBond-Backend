using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;
using MetaBond.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MetaBond.Infrastructure.Persistence.Repository.Account;

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
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<CommunityUser>> GetByCommunityIdAsync(Guid communityId, CancellationToken cancellationToken)
    {
        return await _metaBondContext.Set<CommunityUser>()
            .AsNoTracking()
            .Where(x => x.CommunityId == communityId)
            .Include(us => us.User)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<CommunityUser>> GetCommunitiesByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _metaBondContext.Set<CommunityUser>()
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Include(x => x.Community)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
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
            await DeleteAsync(entity, cancellationToken);
            await SaveAsync(cancellationToken);
        }
        
    }
}