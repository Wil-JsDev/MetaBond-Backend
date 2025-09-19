using MetaBond.Application.Abstractions.Messaging;

namespace MetaBond.Application.Feature.CommunityMembership.Commands.UpdateRole;

public sealed class UpdateCommunityMembershipRoleCommand : ICommand<string>
{
    public Guid? CommunityId { get; set; }

    public Guid? UserId { get; set; }

    public string? Role { get; set; }
}