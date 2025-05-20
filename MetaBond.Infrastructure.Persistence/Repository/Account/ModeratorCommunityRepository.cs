using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Domain.Models;
using MetaBond.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MetaBond.Infrastructure.Persistence.Repository.Account;

public class ModeratorCommunityRepository(MetaBondContext metaBondContext) : GenericRepository<ModeratorCommunity>(metaBondContext),IModeratorCommunityRepository
{
    public async Task<bool> IsModeratorOfCommunityAsync(Guid moderatorId, Guid communityId, CancellationToken cancellationToken)
    {
       return await ValidateAsync(cm => cm.CommunitiesId == communityId && cm.ModeratorId == moderatorId, cancellationToken);
    }

    public async Task<IEnumerable<Communities>> GetCommunitiesByModeratorIdAsync(Guid moderatorId, CancellationToken cancellationToken)
    {
        return await _metaBondContext.Set<ModeratorCommunity>()
            .AsNoTracking()
            .Where(cm => cm.ModeratorId == moderatorId)
            .Include(cm => cm.Community)
            .Select(cm => cm.Community!)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ModeratorCommunity>> GetModeratorCommunitiesAsync(Guid moderatorId, CancellationToken cancellationToken)
    {
        return await _metaBondContext.Set<ModeratorCommunity>()
            .AsNoTracking()
            .Where(moderator => moderator.ModeratorId == moderatorId)
                .Include(moderator => moderator.Community)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
    }

    public async Task<Moderator?> GetModeratorWithDetailsAsync(Guid moderatorId, CancellationToken cancellationToken)
    {
        return await _metaBondContext.Set<Moderator>()
            .AsNoTracking()
            .Where(us =>  us.Id == moderatorId)
                .Include(us => us.User)
                .Include(cm => cm.ModeratorCommunities)
            .AsSplitQuery()
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task RemoveModeratorFromCommunityAsync(Guid moderatorId, Guid communityId, CancellationToken cancellationToken)
    {
        var entity = await _metaBondContext.Set<ModeratorCommunity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(cm => cm.ModeratorId == moderatorId && cm.CommunitiesId == communityId, cancellationToken);

        if (entity != null)
        {
            _metaBondContext.Set<ModeratorCommunity>().Remove(entity);
            await SaveAsync(cancellationToken);
        }
    }

    public async Task<bool> ModeratorCommunityExistsAsync(Guid moderatorId, Guid communityId, CancellationToken cancellationToken)
    {
       return await ValidateAsync(cm => cm.CommunitiesId == communityId && cm.ModeratorId == moderatorId, cancellationToken);
    }
}