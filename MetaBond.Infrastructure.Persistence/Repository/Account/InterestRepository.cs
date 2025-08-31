using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;
using MetaBond.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MetaBond.Infrastructure.Persistence.Repository.Account;

public class InterestRepository(MetaBondContext metaBondContext)
    : GenericRepository<Interest>(metaBondContext), IInterestRepository
{
    public async Task<PagedResult<Interest>> GetPagedInterestAsync(int pageNumber, int pageSize,
        CancellationToken cancellationToken)
    {
        var total = await _metaBondContext.Set<Interest>().AsNoTracking().CountAsync(cancellationToken);

        var query = await _metaBondContext.Set<Interest>()
            .AsNoTracking()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Interest>(query, total, pageNumber, pageSize);
    }

    public async Task<PagedResult<Interest>> GetInterestsByNameAsync(
        string interestName,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var total = await _metaBondContext
            .Set<Interest>()
            .CountAsync(i => EF.Functions.ILike(i.Name!, $"%{interestName}%"), cancellationToken);

        var query = await _metaBondContext.Set<Interest>()
            .AsNoTracking()
            .Where(i => EF.Functions.ILike(i.Name!, $"%{interestName}%"))
            .Include(us => us.UserInterests)!
            .ThenInclude(us => us.User)
            .AsSplitQuery()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Interest>(query, total, pageNumber, pageSize);
    }

    public async Task<PagedResult<Interest>> GetInterestsByUserAsync(Guid userId, int pageNumber, int pageSize,
        CancellationToken cancellationToken)
    {
        var total = await _metaBondContext.Set<Interest>()
            .AsNoTracking()
            .Where(i => i.UserInterests!.Any(ui => ui.UserId == userId))
            .CountAsync(cancellationToken);

        var query = await _metaBondContext.Set<Interest>()
            .AsNoTracking()
            .Where(i => i.UserInterests!.Any(ui => ui.UserId == userId))
            .ToListAsync(cancellationToken);

        return new PagedResult<Interest>(query, total, total, total);
    }

    public async Task<bool> InterestExistsAsync(string interestName, CancellationToken cancellationToken)
    {
        return await ValidateAsync(i => EF.Functions.ILike(i.Name!, interestName), cancellationToken);
    }
}