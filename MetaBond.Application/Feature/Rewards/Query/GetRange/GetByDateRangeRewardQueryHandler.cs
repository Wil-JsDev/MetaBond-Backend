using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Rewards;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Rewards.Query.GetRange;

internal sealed class GetByDateRangeRewardQueryHandler(
    IRewardsRepository rewardsRepository,
    ILogger<GetByDateRangeRewardQueryHandler> logger)
    : IQueryHandler<GetByDateRangeRewardQuery, IEnumerable<RewardsDTos>>
{
    public async Task<ResultT<IEnumerable<RewardsDTos>>> Handle(
        GetByDateRangeRewardQuery request,
        CancellationToken cancellationToken)
    {
        var rewards = GetValue();
        if (rewards.TryGetValue((request.Range), out var dateRange))
        {
            var rewardsList = await dateRange(cancellationToken);
            IEnumerable<Domain.Models.Rewards> rewardsEnumerable = rewardsList.ToList();
            if (rewardsList == null || !rewardsEnumerable.Any())
            {
                logger.LogError("No rewards found for the given date range: {Range}", request.Range);

                return ResultT<IEnumerable<RewardsDTos>>.Failure(Error.Failure("400","The list is empty"));
            }

            IEnumerable<RewardsDTos> rewardsDTos = rewardsEnumerable.Select(x => new RewardsDTos
            (
                RewardsId: x.Id,
                Description: x.Description,
                PointAwarded: x.PointAwarded,
                DateAwarded: x.DateAwarded
            ));

            IEnumerable<RewardsDTos> value = rewardsDTos.ToList();
            logger.LogInformation("Successfully retrieved {Count} rewards for date range: {Range}", 
                value.Count(), 
                request.Range);

            return ResultT<IEnumerable<RewardsDTos>>.Success(value);
        }
        logger.LogError("Invalid date range: {Range}. No matching rewards found.", request.Range);

        return ResultT<IEnumerable<RewardsDTos>>.Failure(Error.Failure("400", "Invalid date range"));
    }

    #region Private Methods
    
    private Dictionary<DateRangeType, Func<CancellationToken, Task<IEnumerable<Domain.Models.Rewards>>>> GetValue()
    {
        return new Dictionary<DateRangeType, Func<CancellationToken, Task<IEnumerable<Domain.Models.Rewards>>>>
        {
            { DateRangeType.Today,
                async cancellationToken =>
                    await rewardsRepository.GetRewardsByDateRangeAsync(
                        DateTime.UtcNow.Date,
                        DateTime.UtcNow.AddDays(1).AddTicks(-1),
                        cancellationToken
                    )},
            { DateRangeType.Week,
                async cancellationToken =>
                    await rewardsRepository.GetRewardsByDateRangeAsync(
                        DateTime.UtcNow.Date.AddDays(-7),
                        DateTime.UtcNow.Date.AddTicks(-7),
                        cancellationToken
                    )},

            { DateRangeType.Month,
                async cancellationToken =>
                    await rewardsRepository.GetRewardsByDateRangeAsync(
                        new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1),
                        new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(1).AddTicks(-1),
                        cancellationToken
                    )},

            { DateRangeType.Year,
                async cancellationToken =>
                    await rewardsRepository.GetRewardsByDateRangeAsync(
                        new DateTime(DateTime.UtcNow.Year, 1, 1),
                        new DateTime(DateTime.UtcNow.Year + 1, 1, 1).AddTicks(-1),
                        cancellationToken
                    )},
        };   
    }
    
    #endregion
    
}