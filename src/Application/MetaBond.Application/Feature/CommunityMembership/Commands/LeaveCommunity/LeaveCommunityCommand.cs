using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.CommunityMembership;

namespace MetaBond.Application.Feature.CommunityMembership.Commands.LeaveCommunity;

public sealed class LeaveCommunityCommand : ICommand<LeaveCommunityDto>
{
    public Guid? CommunityId { get; set; }

    public Guid? UserId { get; set; }
}