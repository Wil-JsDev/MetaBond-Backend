using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.CommunityMembership;

namespace MetaBond.Application.Feature.CommunityMembership.Commands.JoinCommunity;

public sealed class JoinCommunityCommand : ICommand<CommunityMembershipDto>
{
    public Guid? CommunityId { get; set; }

    public Guid? UserId { get; set; }
}