using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Domain.Models;
using MetaBond.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MetaBond.Infrastructure.Persistence.Repository.Account;

public class RoleRepository(MetaBondContext metaBondContext) : IRoleRepository
{
    public async Task<Roles?> GetByIdAsync(Guid rolId, CancellationToken cancellationToken)
    {
        return await metaBondContext.Set<Roles>()
            .FirstOrDefaultAsync(r => r.Id == rolId, cancellationToken: cancellationToken);
    }

    public async Task<Roles?> GetByNameAsync(string roleName, CancellationToken cancellationToken)
    {
        return await metaBondContext.Set<Roles>()
            .FirstOrDefaultAsync(r => r.Name == roleName, cancellationToken: cancellationToken);
    }
}