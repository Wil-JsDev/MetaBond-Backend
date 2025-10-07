using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.DTOs.Communities;

namespace MetaBond.Application.DTOs.Account.CommunityMembership;

public sealed record CommunityMembershipDto(
    Guid CommunityMembershipId,
    UserCommunityMembershipDto User,
    CommunitiesDTos Community,
    string? Role,
    bool IsActive
);