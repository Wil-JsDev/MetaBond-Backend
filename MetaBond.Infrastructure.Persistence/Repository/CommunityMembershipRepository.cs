using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;
using MetaBond.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MetaBond.Infrastructure.Persistence.Repository;

public class CommunityMembershipRepository(MetaBondContext metaBondContext)
    : GenericRepository<CommunityMembership>(metaBondContext), ICommunityMembershipRepository
{
    public async Task<PagedResult<CommunityMembership>> GetCommunityMembersAsync(Guid communityId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = await _metaBondContext.Set<CommunityMembership>()
            .AsNoTracking()
            .OrderBy(cm => cm.UserId)
            .Where(x => x.CommunityId == communityId)
            .Include(x => x.User)
            .ThenInclude(us => us!.Interests)!
            .ThenInclude(interest => interest.Interest!.Name)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        var total = query.Count;

        var pagedResponse = new PagedResult<CommunityMembership>(query, pageNumber, pageSize, total);
        return pagedResponse;
    }

    public async Task<CommunityMembership> LeaveCommunityAsync(Guid userId, Guid communityId,
        CancellationToken cancellationToken)
    {
        var communityMembership = await _metaBondContext.Set<CommunityMembership>()
            .AsNoTracking()
            .Where(cm => cm.UserId == userId && cm.CommunityId == communityId)
            .FirstOrDefaultAsync(cancellationToken);

        communityMembership!.IsActive = false;
        communityMembership.LeftOnUtc = DateTime.UtcNow;
        communityMembership.User = null;
        communityMembership.Community = null;
        communityMembership.Role = null;

        await UpdateAsync(communityMembership, cancellationToken);

        return communityMembership;
    }

    public async Task<CommunityMembership> JoinCommunityAsync(CommunityMembership entity,
        CancellationToken cancellationToken)
    {
        await _metaBondContext.Set<CommunityMembership>().AddAsync(entity, cancellationToken);
        await _metaBondContext.SaveChangesAsync(cancellationToken);

        return await _metaBondContext.Set<CommunityMembership>()
            .Include(cm => cm.User)
            .Include(cm => cm.Community)
            .FirstAsync(cm => cm.UserId == entity.UserId && cm.CommunityId == entity.CommunityId, cancellationToken);
    }

    public async Task<string?> GetUserRoleAsync(Guid userId, Guid communityId, CancellationToken cancellationToken)
    {
        var communityMembership = await _metaBondContext.Set<CommunityMembership>()
            .AsNoTracking()
            .Where(cm => cm.UserId == userId && cm.CommunityId == communityId)
            .FirstOrDefaultAsync(cancellationToken);

        return communityMembership?.Role;
    }

    public async Task<PagedResult<CommunityMembership>> GetUserCommunitiesAsync(Guid userId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = await _metaBondContext.Set<CommunityMembership>()
            .AsNoTracking()
            .OrderBy(cm => cm.CommunityId)
            .Where(x => x.UserId == userId)
            .Include(x => x.Community)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        var count = query.Count;

        var pagedResponse = new PagedResult<CommunityMembership>(query, pageNumber, pageSize, count);
        return pagedResponse;
    }

    public async Task<bool> IsUserMemberAsync(Guid userId, Guid communityId, CancellationToken cancellationToken)
    {
        return await ValidateAsync(cm => cm.UserId == userId && cm.CommunityId == communityId, cancellationToken);
    }

    public async Task UpdateUserRoleAsync(Guid userId, Guid communityId, string role,
        CancellationToken cancellationToken)
    {
        var communityMembership = await _metaBondContext.Set<CommunityMembership>()
            .AsNoTracking()
            .Where(cm => cm.UserId == userId && cm.CommunityId == communityId)
            .FirstOrDefaultAsync(cancellationToken);

        communityMembership!.Role = role;

        await UpdateAsync(communityMembership, cancellationToken);
    }
}