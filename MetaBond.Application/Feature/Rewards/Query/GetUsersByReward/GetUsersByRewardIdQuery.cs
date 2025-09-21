using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.DTOs.Rewards;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.Rewards.Query.GetUsersByReward;

public sealed class GetUsersByRewardIdQuery : IQuery<PagedResult<RewardsWithUserDTos>>
{
    public Guid RewardsId { get; init; }
    
    public int PageNumber { get; init; }
    
    public int PageSize { get; init; }
}