using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Rewards;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Rewards.Querys.GetRange
{
    internal sealed class GetByDateRangeRewardQueryHandler : IQueryHandler<GetByDateRangeRewardQuery, IEnumerable<RewardsDTos>>
    {
        private readonly IRewardsRepository _rewardsRepository;
        private readonly ILogger<GetByDateRangeRewardQueryHandler> _logger;

        public GetByDateRangeRewardQueryHandler(
            IRewardsRepository rewardsRepository,
            ILogger<GetByDateRangeRewardQueryHandler> logger)
        {
            _rewardsRepository = rewardsRepository;
            _logger = logger;
        }

        public async Task<ResultT<IEnumerable<RewardsDTos>>> Handle(
            GetByDateRangeRewardQuery request,
            CancellationToken cancellationToken)
        {
            var rewards = GetValue();
            if (rewards.TryGetValue((request.Range), out var dateRange))
            {
                var rewardsList = await dateRange(cancellationToken);
                if (rewardsList == null || !rewardsList.Any())
                {
                    _logger.LogError("No rewards found for the given date range: {Range}", request.Range);

                    return ResultT<IEnumerable<RewardsDTos>>.Failure(Error.Failure("400","The list is empty"));
                }

                IEnumerable<RewardsDTos> rewardsDTos = rewardsList.Select(x => new RewardsDTos
                (
                    RewardsId: x.Id,
                    Description: x.Description,
                    PointAwarded: x.PointAwarded,
                    DateAwarded: x.DateAwarded
                ));

                _logger.LogInformation("Successfully retrieved {Count} rewards for date range: {Range}", 
                    rewardsDTos.Count(), 
                    request.Range);

                return ResultT<IEnumerable<RewardsDTos>>.Success(rewardsDTos);
            }
            _logger.LogError("Invalid date range: {Range}. No matching rewards found.", request.Range);

            return ResultT<IEnumerable<RewardsDTos>>.Failure(Error.Failure("400", "Invalid date range"));
        }

        private Dictionary<DateRangeType, Func<CancellationToken, Task<IEnumerable<Domain.Models.Rewards>>>> GetValue()
        {
            return new Dictionary<DateRangeType, Func<CancellationToken, Task<IEnumerable<Domain.Models.Rewards>>>>
            {
                { DateRangeType.Today,
                    async cancellationToken =>
                        await _rewardsRepository.GetRewardsByDateRangeAsync(
                            DateTime.UtcNow.Date,
                            DateTime.UtcNow.AddDays(1).AddTicks(-1),
                            cancellationToken
                        )},
                { DateRangeType.Week,
                    async cancellationToken =>
                        await _rewardsRepository.GetRewardsByDateRangeAsync(
                             DateTime.UtcNow.Date.AddDays(-7),
                             DateTime.UtcNow.Date.AddTicks(-7),
                             cancellationToken
                        )},

                { DateRangeType.Month,
                    async cancellationToken =>
                        await _rewardsRepository.GetRewardsByDateRangeAsync(
                            new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1),
                            new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(1).AddTicks(-1),
                            cancellationToken
                        )},

                { DateRangeType.Year,
                    async cancellationToken =>
                        await _rewardsRepository.GetRewardsByDateRangeAsync(
                            new DateTime(DateTime.UtcNow.Year, 1, 1),
                            new DateTime(DateTime.UtcNow.Year + 1, 1, 1).AddTicks(-1),
                            cancellationToken
                        )},
            };   
        }
    }
}
