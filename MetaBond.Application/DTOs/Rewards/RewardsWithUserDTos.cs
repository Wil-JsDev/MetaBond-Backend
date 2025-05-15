using MetaBond.Application.DTOs.Account.User;

namespace MetaBond.Application.DTOs.Rewards;

public sealed record RewardsWithUserDTos
(
    Guid? RewardsId,
    UserRewardsDTos User,
    string? Description,
    int? PointAwarded,
    DateTime? DateAwarded
);