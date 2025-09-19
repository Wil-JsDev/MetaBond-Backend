using MetaBond.Application.DTOs.Account.Roles;
using MetaBond.Application.Utils;

namespace MetaBond.Application.Interfaces.Service;

public interface IRoleService
{
    Task<ResultT<RolesDto>> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken);

    Task<ResultT<RolesDto>> GetRoleByIdAsync(Guid roleId, CancellationToken cancellationToken);
}