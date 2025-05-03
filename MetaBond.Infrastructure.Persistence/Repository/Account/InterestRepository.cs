using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Domain.Models;
using MetaBond.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MetaBond.Infrastructure.Persistence.Repository.Account;

public class InterestRepository(MetaBondContext metaBondContext)
    : GenericRepository<Interest>(metaBondContext), IInterestRepository
{
    public async Task<IEnumerable<Interest>> GetInterestsByNameAsync(string interestName,CancellationToken cancellationToken)
    {
        return await _metaBondContext.Set<Interest>()
            .AsNoTracking()
            .Where(x => x.Name!.Contains(interestName, StringComparison.InvariantCultureIgnoreCase))
            .Include(us => us.UserInterests)!
                .ThenInclude(us => us.User)
            .ToListAsync(cancellationToken);
    }
}