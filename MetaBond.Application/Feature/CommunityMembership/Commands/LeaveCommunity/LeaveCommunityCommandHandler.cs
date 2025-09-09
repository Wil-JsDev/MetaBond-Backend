using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.CommunityMembership;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.CommunityMembership.Commands.LeaveCommunity;

internal sealed class LeaveCommunityCommandHandler(
    ILogger<LeaveCommunityCommandHandler> logger,
    ICommunityMembershipRepository communityMembershipRepository,
    IUserRepository userRepository,
    ICommunitiesRepository communitiesRepository
) : ICommandHandler<LeaveCommunityCommand, LeaveCommunityDto>
{
    public async Task<ResultT<LeaveCommunityDto>> Handle(LeaveCommunityCommand request,
        CancellationToken cancellationToken)
    {
        var userResult = await EntityHelper.GetEntityByIdAsync(userRepository.GetByIdAsync,
            request.UserId ?? Guid.Empty, "User", logger);

        if (!userResult.IsSuccess) return ResultT<LeaveCommunityDto>.Failure(userResult.Error!);

        var communityResult = await EntityHelper.GetEntityByIdAsync(communitiesRepository.GetByIdAsync,
            request.CommunityId ?? Guid.Empty, "Community", logger);

        if (!communityResult.IsSuccess) return ResultT<LeaveCommunityDto>.Failure(communityResult.Error!);

        if (!await communityMembershipRepository.IsUserMemberAsync(request.UserId!.Value, request.CommunityId!.Value,
                cancellationToken))
        {
            logger.LogWarning("User {UserId} is not a member of Community {CommunityId}.", request.UserId,
                request.CommunityId);

            return ResultT<LeaveCommunityDto>.Failure(Error.Failure("404",
                $"User {request.UserId} is not a member of Community {request.CommunityId}."));
        }

        var leaveCommunity = await communityMembershipRepository.LeaveCommunityAsync(request.UserId ?? Guid.Empty,
            request.CommunityId ?? Guid.Empty, cancellationToken);

        logger.LogInformation("User {UserId} successfully left Community {CommunityId}.", request.UserId,
            request.CommunityId);

        return ResultT<LeaveCommunityDto>.Success(
            CommunityMembershipMapper.LeaveCommunityToDto(leaveCommunity)
        );
    }
}