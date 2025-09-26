using System.Reflection.Metadata;
using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.Auth;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Interfaces.Service.Auth;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;
using Serilog;

namespace MetaBond.Application.Feature.Authentication.Commands.GenerateCommunityToken;

internal sealed class GenerateCommunityTokenCommandHandler(
    ILogger<GenerateCommunityTokenCommandHandler> logger,
    IJwtService jwtService,
    IUserRepository userRepository,
    ICommunitiesRepository communitiesRepository,
    ICommunityMembershipRepository communityMembershipRepository
) : ICommandHandler<GenerateCommunityTokenCommand, AuthenticationCommunityResponse>
{
    public async Task<ResultT<AuthenticationCommunityResponse>> Handle(GenerateCommunityTokenCommand request,
        CancellationToken cancellationToken)
    {
        var user = await EntityHelper.GetEntityByIdAsync(
            userRepository.GetByIdAsync,
            request.UserId,
            "User",
            logger
        );

        if (!user.IsSuccess)
            return user.Error;

        var community = await EntityHelper.GetEntityByIdAsync(
            communitiesRepository.GetByIdAsync,
            request.CommunityId,
            "Community",
            logger
        );

        if (!community.IsSuccess)
            return community.Error;

        var communityMembership = await communityMembershipRepository.GetByUserAndCommunityAsync(
            request.UserId,
            request.CommunityId,
            cancellationToken
        );

        if (communityMembership is null || !communityMembership.IsActive)
        {
            Log.Error("User is not a member of this community");

            return ResultT<AuthenticationCommunityResponse>.Failure(
                Error.Failure("403", "User is not a member of this community"));
        }

        var communityMembershipToken =
            jwtService.GenerateTokenCommunity(user.Value, communityMembership,
                communityMembership.Role ?? string.Empty);

        logger.LogInformation("User {UserId} successfully logged in Community {CommunityId}.", community.Value.Id,
            user.Value.Id);

        return ResultT<AuthenticationCommunityResponse>.Success(
            new AuthenticationCommunityResponse(communityMembershipToken));
    }
}