using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;
using MetaBond.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MetaBond.Infrastructure.Persistence.Repository.Account;

public class CommunityManagerRepository(MetaBondContext metaBondContext)
    : GenericRepository<CommunityManager>(metaBondContext), ICommunityManagerRepository
{
    
    public async Task<PagedResult<CommunityManager>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken)
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
        
        return new PagedResult<CommunityManager>(pagedCommunityManager, page, pageSize, totalRecord);
    }

    public async Task<bool> IsUserCommunityManagerAsync(Guid userId, Guid communityId, CancellationToken cancellationToken)
    {
        return await ValidateAsync(cm => cm.CommunityId == communityId && cm.UserId == userId, cancellationToken);
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
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountCommunityManagersAsync(Guid communityId, CancellationToken cancellationToken)
    {
        return await _metaBondContext.Set<CommunityManager>()
            .AsNoTracking()
            .CountAsync(cm => cm.CommunityId == communityId ,cancellationToken);
    }

    public async Task<bool> IsManagerAssignedToCommunityAsync(Guid communityId, CancellationToken cancellationToken)
    {
        return await ValidateAsync(cm => cm.CommunityId == communityId, cancellationToken);
    }

    public async Task<bool> DoesManagerExistAsync(Guid managerId, CancellationToken cancellationToken)
    {
        return await ValidateAsync(cm => cm.UserId == managerId, cancellationToken);
    }
}