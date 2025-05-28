using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.DTOs.Rewards;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Mapper;

public static class RewardsMapper
{
    public static RewardsWithUserDTos ToDto(Rewards x)
    {
        return new RewardsWithUserDTos(

            RewardsId: x.Id,
            User: (x.User != null ? new UserRewardsDTos(
                UserId: x.User!.Id,
                FirstName: x.User.FirstName,
                LastName: x.User.LastName
            ) : null)!,
            Description: x.Description,
            PointAwarded: x.PointAwarded,
            DateAwarded: x.DateAwarded
        );
    }
}