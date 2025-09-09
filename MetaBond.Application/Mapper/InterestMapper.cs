using MetaBond.Application.DTOs.Interest;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Mapper;

public static class InterestMapper
{
    public static InterestDTos ModelToDto(Interest interest)
    {
        return new InterestDTos
        (
            InterestId: interest.Id,
            Name: interest.Name,
            InterestCategoryId: interest.InterestCategoryId
        );
    }

    public static InterestWithUserDto UserWithUserDto(Interest interest)
    {
        return new InterestWithUserDto
        (
            Users: interest.UserInterests?
                .Select(ui => UserMapper.MapUserDTos(ui.User))
                .ToList() ?? []
        );
    }
}