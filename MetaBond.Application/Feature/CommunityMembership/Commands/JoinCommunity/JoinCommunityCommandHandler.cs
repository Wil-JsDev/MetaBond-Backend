using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.CommunityMembership;
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

        var community = await communitiesRepository.GetByIdAsync(request.CommunityId ?? Guid.Empty);
        if (community is null)
        {
            logger.LogError("Community with ID {CommunityId} was not found when processing JoinCommunityCommand.",
                request.CommunityId);

            return ResultT<CommunityMembershipDto>.Failure(
                Error.NotFound("404", $"Community with ID {request.CommunityId} was not found.")
            );
        }

        var user = await userRepository.GetByIdAsync(request.UserId ?? Guid.Empty);
        if (user is null)
        {
            logger.LogError("User with ID {UserId} was not found when processing JoinCommunityCommand.",
                request.UserId);

            return ResultT<CommunityMembershipDto>.Failure(
                Error.NotFound("404", $"User with ID {request.UserId} was not found.")
            );
        }

        var communityMembership = new Domain.Models.CommunityMembership
        {
            CommunityId = community.Id,
            UserId = user.Id,
            Role = CommunityMembershipRoles.Member.ToString()
        };

        await communityMembershipRepository.JoinCommunityAsync(communityMembership, cancellationToken);

        logger.LogInformation("User {UserId} successfully joined Community {CommunityId}.", user.Id, community.Id);

        var communityMembershipDto = CommunityMembershipMapper.ModelToDto(communityMembership);

        return ResultT<CommunityMembershipDto>.Success(communityMembershipDto);
    }
}