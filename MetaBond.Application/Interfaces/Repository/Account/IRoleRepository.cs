using MetaBond.Domain.Models;

namespace MetaBond.Application.Interfaces.Repository.Account;

public interface IRoleRepository
{
    Task<Roles?> GetByIdAsync(Guid rolId, CancellationToken cancellationToken);

    Task<Roles?> GetByNameAsync(string roleName, CancellationToken cancellationToken);
}