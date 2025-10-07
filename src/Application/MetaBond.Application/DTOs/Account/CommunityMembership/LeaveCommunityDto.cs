namespace MetaBond.Application.DTOs.Account.CommunityMembership;

public sealed record LeaveCommunityDto(
    bool IsActive,
    DateTime? LeftOnUtc
);