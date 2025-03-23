using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Rewards;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Rewards.Querys.GetRecent
{
    internal sealed class GetMostRecentRewardsQueryHandler : IQueryHandler<GetMostRecentRewardsQuery, RewardsDTos>
    {
        private readonly IRewardsRepository _rewardsRepository;
        private readonly ILogger<GetMostRecentRewardsQueryHandler> _logger;

        public GetMostRecentRewardsQueryHandler(
            IRewardsRepository rewardsRepository, 
            ILogger<GetMostRecentRewardsQueryHandler> logger)
        {
            _rewardsRepository = rewardsRepository;
            _logger = logger;
        }

        public async Task<ResultT<RewardsDTos>> Handle(
            GetMostRecentRewardsQuery request, 
            CancellationToken cancellationToken)
        {
            var rewardRecent = await _rewardsRepository.GetMostRecentRewardAsync(cancellationToken);
            if (rewardRecent != null)
            {
                _logger.LogInformation("Most recent reward found with ID: {RewardsId}", rewardRecent.Id);

                RewardsDTos rewardsDTos = new
                (
                    RewardsId: rewardRecent.Id,
                    Description: rewardRecent.Description,
                    PointAwarded: rewardRecent.PointAwarded,
                    DateAwarded: rewardRecent.DateAwarded
                );
                
                return ResultT<RewardsDTos>.Success(rewardsDTos);

            }
            _logger.LogWarning("No recent rewards found.");

            return ResultT<RewardsDTos>.Failure(Error.Failure("400", "No rewards available"));
        }
    }
}
