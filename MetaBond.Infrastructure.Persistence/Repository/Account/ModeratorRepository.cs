using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Domain.Models;
using MetaBond.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MetaBond.Infrastructure.Persistence.Repository.Account;

public class ModeratorRepository(MetaBondContext metaBondContext)
    : GenericRepository<Moderator>(metaBondContext), IModeratorRepository
{
    public async Task<Moderator?> GetWithUserAsync(Guid moderatorId)
    {
        return await _metaBondContext.Set<Moderator>()
            .AsNoTracking()
            .Where(moderator => moderator.Id == moderatorId)
            .Include(moderator => moderator.User)
            .AsSplitQuery()
            .FirstOrDefaultAsync();
    }
}