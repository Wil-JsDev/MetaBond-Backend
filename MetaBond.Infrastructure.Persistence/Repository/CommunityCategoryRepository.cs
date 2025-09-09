using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;
using MetaBond.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MetaBond.Infrastructure.Persistence.Repository;

public class CommunityCategoryRepository(MetaBondContext metaBondContext)
    : GenericRepository<CommunityCategory>(metaBondContext), ICommunityCategoryRepository
{
    public async Task<bool> ExistsNameAsync(string name, CancellationToken cancellationToken)
    {
        return await ValidateAsync(x => x.Name == name, cancellationToken);
    }

    public async Task<bool> ExistsNameExceptIdAsync(string name, Guid excludeId, CancellationToken cancellationToken)
    {
        return await ValidateAsync(cc => cc.Name == name && cc.Id != excludeId, cancellationToken);
    }

    public async Task<CommunityCategory?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _metaBondContext.Set<CommunityCategory>()
            .AsNoTracking()
            .FirstOrDefaultAsync(cm => cm.Name == name, cancellationToken: cancellationToken);
    }

    public async Task<PagedResult<CommunityCategory>> GetPagedAsync(int pageNumber, int pageSize,
        CancellationToken cancellationToken)
    {
        var total = await _metaBondContext.Set<CommunityCategory>().AsNoTracking().CountAsync(cancellationToken);

        var query = await _metaBondContext.Set<CommunityCategory>()
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        
        var pagedResponse = new PagedResult<CommunityCategory>(query, pageNumber, pageSize, total);

        return pagedResponse;
    }
}