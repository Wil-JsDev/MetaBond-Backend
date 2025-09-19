using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;
using MetaBond.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MetaBond.Infrastructure.Persistence.Repository.Account;

public class AdminRepository(MetaBondContext metaBondContext)
    : GenericRepository<Admin>(metaBondContext), IAdminRepository
{
    public async Task<PagedResult<Admin>> GetPagedAsync(int pageNumber, int pageSize,
        CancellationToken cancellationToken)
    {
        var totalRecord = await _metaBondContext.Set<Admin>()
            .AsNoTracking()
            .CountAsync(cancellationToken);

        var pagedAdmin = await _metaBondContext.Set<Admin>()
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Admin>(pagedAdmin, pageNumber, pageSize, totalRecord);
    }
}