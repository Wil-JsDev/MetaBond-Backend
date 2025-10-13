using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.CommunityMembership.Commands.UpdateRole;

internal sealed class UpdateCommunityMembershipRoleCommandHandler(
    ILogger<UpdateCommunityMembershipRoleCommandHandler> logger,
    ICommunityMembershipRepository communityMembershipRepository,
    ICommunitiesRepository communitiesRepository,
    IUserRepository userRepository
) : ICommandHandler<UpdateCommunityMembershipRoleCommand, string>
{
    public async Task<ResultT<string>> Handle(UpdateCommunityMembershipRoleCommand request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogError("UpdateCommunityMembershipRoleCommand request is null.");

            return ResultT<string>.Failure(Error.Conflict("409", "The update request cannot be null."));
        }

        var communityResult = await EntityHelper.GetEntityByIdAsync(communitiesRepository.GetByIdAsync,
            request.CommunityId ?? Guid.Empty,
            "Community",
            logger
        );
        if (!communityResult.IsSuccess) return ResultT<string>.Failure(communityResult.Error!);

        var userResult = await EntityHelper.GetEntityByIdAsync(userRepository.GetByIdAsync,
            request.UserId ?? Guid.Empty,
            "User",
            logger
        );

        if (!userResult.IsSuccess) return ResultT<string>.Failure(userResult.Error!);

        // member -> Member
        if (!Enum.TryParse(typeof(CommunityMembershipRoles), request.Role, true, out var parsedRole))
        {
            logger.LogWarning(
                "Failed to assign role: '{Role}' is not a valid role for User {UserId} in Community {CommunityId}.",
                request.Role, request.UserId, request.CommunityId);

            return ResultT<string>.Failure(
                Error.Conflict("409",
                    $"The role '{request.Role}' is not valid for User '{userResult.Value.FirstName}' in Community '{communityResult.Value.Name}'."));
        }

        var normalizedRole = parsedRole.ToString();

        var currentRole = await communityMembershipRepository.GetUserRoleAsync(
            userResult.Value.Id,
            communityResult.Value.Id,
            cancellationToken
        );

        if (currentRole is not null && string.Equals(currentRole, normalizedRole, StringComparison.OrdinalIgnoreCase))
        {
            logger.LogWarning(
                "User {UserId} already has role {Role} in Community {CommunityId}. Update not allowed.",
                userResult.Value.Id,
                normalizedRole,
                communityResult.Value.Id
            );

            return ResultT<string>.Failure(
                Error.Conflict("409", $"User already has the role '{normalizedRole}' in this community.")
            );
        }

        await communityMembershipRepository.UpdateUserRoleAsync(
            userResult.Value.Id,
            communityResult.Value.Id,
            normalizedRole!,
            cancellationToken
        );

        logger.LogInformation(
            "User {UserId} successfully updated role to {Role} in Community {CommunityId}.",
            userResult.Value.Id,
            request.Role,
            communityResult.Value.Id
        );

        return ResultT<string>.Success("User role updated successfully.");
    }
}