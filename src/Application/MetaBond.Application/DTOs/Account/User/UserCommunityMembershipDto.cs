namespace MetaBond.Application.DTOs.Account.User;

public sealed record UserCommunityMembershipDto(
    Guid UserId,
    string? Username,
    string? FullName,
    string? Photo
);