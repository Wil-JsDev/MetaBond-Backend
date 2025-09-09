using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;
using MetaBond.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MetaBond.Infrastructure.Persistence.Repository;

public class InterestCategoryRepository(MetaBondContext metaBondContext)
    : GenericRepository<InterestCategory>(metaBondContext), IInterestCategoryRepository
{
    public async Task<bool> ExistsNameAsync(string name, CancellationToken cancellationToken)
    {
        return await ValidateAsync(x => x.Name == name, cancellationToken);
    }

    public async Task<bool> ExistsNameExceptIdAsync(string name, Guid excludeId, CancellationToken cancellationToken)
    {
        return await ValidateAsync(x => x.Name == name && x.Id != excludeId, cancellationToken);
    }

    public async Task<InterestCategory?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _metaBondContext.Set<InterestCategory>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken: cancellationToken);
    }

    public async Task<PagedResult<InterestCategory>> GetPagedAsync(int pageNumber, int pageSize,
        CancellationToken cancellationToken)
    {
        var total = await _metaBondContext.Set<InterestCategory>().AsNoTracking().CountAsync(cancellationToken);

        var query = await _metaBondContext.Set<InterestCategory>()
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var pagedResponse = new PagedResult<InterestCategory>(query, pageNumber, pageSize, total);

        return pagedResponse;
    }
}