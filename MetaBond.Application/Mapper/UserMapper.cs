using MetaBond.Application.DTOs.Account.User;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Mapper;

public static class UserMapper
{
    public static UserDTos MapUserDTos(User? user)
    {
        return new UserDTos(
            UserId: user!.Id,
            FirstName: user.FirstName,
            LastName: user.LastName,
            Username: user.Username,
            Photo: user.Photo,
            StatusAccount: user.StatusUser,
            CreatedAt: user.CreatedAt,
            UpdateAt: user.UpdatedAt
        );
    }
}