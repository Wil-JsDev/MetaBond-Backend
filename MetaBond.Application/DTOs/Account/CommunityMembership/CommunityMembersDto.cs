using MetaBond.Application.DTOs.Account.User;

namespace MetaBond.Application.DTOs.Account.CommunityMembership;

public sealed record CommunityMembersDto(
    UserDTos User,
    string? Role
);