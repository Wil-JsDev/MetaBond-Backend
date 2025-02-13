
namespace MetaBond.Application.DTOs.Rewards
{
    public sealed record RewardsDTos
    (
        Guid? RewardsId,
        string? Description,
        int? PointAwarded,
        DateTime? DateAwarded
    );
}
