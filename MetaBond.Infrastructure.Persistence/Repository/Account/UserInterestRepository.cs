using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Domain.Models;
using MetaBond.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MetaBond.Infrastructure.Persistence.Repository.Account;

public class UserInterestRepository(MetaBondContext metaBondContext)
    : GenericRepository<UserInterest>(metaBondContext), IUserInterestRepository
{
    public async Task CreateMultipleUserInterestAsync(List<UserInterest> userInterests,
        CancellationToken cancellationToken)
    {
        await _metaBondContext.Set<UserInterest>().AddRangeAsync(userInterests, cancellationToken);
        await SaveAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<UserInterest>> AssociateInterestsToUserAsync(Guid userId,
        IEnumerable<Guid> interestIds,
        CancellationToken cancellationToken)
    {
        var interestIdSet = new HashSet<Guid>(interestIds);

        var existingInterestIds = await _metaBondContext.Set<UserInterest>()
            .AsNoTracking()
            .Where(ui => ui.UserId == userId && interestIdSet.Contains(ui.InterestId))
            .Select(ui => ui.InterestId)
            .ToListAsync(cancellationToken);

        var newInterestIds = interestIdSet.Except(existingInterestIds).ToList();

        if (!newInterestIds.Any())
            return [];

        var newAssociations = newInterestIds.Select(interest => new UserInterest
        {
            UserId = userId,
            InterestId = interest
        }).ToList();

        await _metaBondContext.Set<UserInterest>().AddRangeAsync(newAssociations, cancellationToken);
        await SaveAsync(cancellationToken);

        return newAssociations;
    }
}