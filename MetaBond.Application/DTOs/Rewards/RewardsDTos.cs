
namespace MetaBond.Application.DTOs.Rewards;
public sealed record RewardsDTos
(
    Guid? RewardsId,
    Guid UserId,
    string? Description,
    int? PointAwarded,
    DateTime? DateAwarded
);