using MetaBond.Application.DTOs.Account.Admin;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Mapper;

public static class AdminMapper
{
    public static AdminDto ToDTo(Admin admin)
    {
        return new AdminDto(
            admin.Id,
            admin.FirstName,
            admin.LastName,
            admin.Username,
            admin.Photo
        );
    }

    public static BanUserResultDto BandUserToDTo(User user)
    {
        return new BanUserResultDto(
            user.Id,
            user.Username,
            user.StatusUser,
            user.BannedAt ?? DateTime.UtcNow
        );
    }

    public static UnbanUserResultDto UnbanUserToDto(User user)
    {
        return new UnbanUserResultDto(
            UserId: user.Id,
            Username: user.Username,
            StatusUser: user.StatusUser
        );
    }
}