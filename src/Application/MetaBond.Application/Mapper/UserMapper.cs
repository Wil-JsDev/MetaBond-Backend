using MetaBond.Application.DTOs.Account.User;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Mapper;

public static class UserMapper
{
    public static UserDTos MapUserDTos(User? user)
    {
        return new UserDTos(
            UserId: user!.Id,
            FullName: $"{user.FirstName} {user.LastName}",
            Username: user.Username,
            Photo: user.Photo,
            StatusAccount: user.StatusUser,
            CreatedAt: user.CreatedAt,
            UpdateAt: user.UpdatedAt
        );
    }

    public static UserCommunityMembershipDto MapUserCommunityMembershipDto(User? user)
    {
        return new UserCommunityMembershipDto(
            UserId: user!.Id,
            Username: user.Username,
            FullName: $"{user.FirstName} {user.LastName}",
            Photo: user.Photo
        );
    }
}