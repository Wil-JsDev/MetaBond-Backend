using MetaBond.Application.DTOs.Account.Roles;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Interfaces.Service;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Services;

public class RoleService(
    ILogger<RoleService> logger,
    IRoleRepository roleRepository
) : IRoleService
{
    public async Task<ResultT<RolesDto>> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(roleName))
        {
            logger.LogWarning("GetRoleByNameAsync: Role name is null or empty.");

            return ResultT<RolesDto>.Failure(Error.Failure("400", "Role name is null or empty."));
        }

        var role = await roleRepository.GetByNameAsync(roleName, cancellationToken);
        if (role is null)
        {
            logger.LogWarning("GetRoleByNameAsync: Role with name '{RoleName}' not found.", roleName);

            return ResultT<RolesDto>.Failure(Error.NotFound("404", "Role not found"));
        }

        RolesDto roles = new(role.Id, role.Name, role.Description);

        logger.LogInformation("GetRoleByNameAsync: Role '{RoleName}' retrieved successfully.", roleName);

        return ResultT<RolesDto>.Success(roles);
    }

    public async Task<ResultT<RolesDto>> GetRoleByIdAsync(Guid roleId, CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetByIdAsync(roleId, cancellationToken);
        if (role is null)
        {
            logger.LogWarning("GetRoleByIdAsync: Role with ID '{RoleId}' not found.", roleId);

            return ResultT<RolesDto>.Failure(Error.NotFound("404", "Role not found"));
        }

        RolesDto roles = new(role.Id, role.Name, role.Description);

        logger.LogInformation("GetRoleByIdAsync: Role '{RoleId}' retrieved successfully.", roleId);

        return ResultT<RolesDto>.Success(roles);
    }
}