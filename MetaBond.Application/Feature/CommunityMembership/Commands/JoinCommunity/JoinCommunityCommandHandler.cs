using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.CommunityMembership;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.CommunityMembership.Commands.JoinCommunity;

internal sealed class JoinCommunityCommandHandler(
    ILogger<JoinCommunityCommandHandler> logger,
    ICommunityMembershipRepository communityMembershipRepository,
    ICommunitiesRepository communitiesRepository,
    IUserRepository userRepository
) : ICommandHandler<JoinCommunityCommand, CommunityMembershipDto>
{
    public async Task<ResultT<CommunityMembershipDto>> Handle(JoinCommunityCommand request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            logger.LogError("JoinCommunityCommand request is null.");

            return ResultT<CommunityMembershipDto>.Failure(
                Error.Failure("400", "Invalid request: JoinCommunityCommand cannot be null.")
            );
        }

        var communityResult = await EntityHelper.GetEntityByIdAsync(communitiesRepository.GetByIdAsync,
            request.CommunityId ?? Guid.Empty,
            "Community",
            logger
        );
        if (!communityResult.IsSuccess) return ResultT<CommunityMembershipDto>.Failure(communityResult.Error!);

        var userResult = await EntityHelper.GetEntityByIdAsync(userRepository.GetByIdAsync,
            request.UserId ?? Guid.Empty,
            "User",
            logger
        );

        if (!userResult.IsSuccess) return ResultT<CommunityMembershipDto>.Failure(userResult.Error!);

        var communityMembership = new Domain.Models.CommunityMembership
        {
            CommunityId = communityResult.Value.Id,
            UserId = userResult.Value.Id,
            Role = CommunityMembershipRoles.Member.ToString()
        };

        await communityMembershipRepository.JoinCommunityAsync(communityMembership, cancellationToken);

        logger.LogInformation("User {UserId} successfully joined Community {CommunityId}.", communityResult.Value.Id,
            userResult.Value.Id);

        var communityMembershipDto = CommunityMembershipMapper.ModelToDto(communityMembership);

        return ResultT<CommunityMembershipDto>.Success(communityMembershipDto);
    }
}