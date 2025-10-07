using MetaBond.Application.DTOs.Account.User;

namespace MetaBond.Application.DTOs.Interest;

public record InterestWithUserDto(
    List<UserDTos> Users
);