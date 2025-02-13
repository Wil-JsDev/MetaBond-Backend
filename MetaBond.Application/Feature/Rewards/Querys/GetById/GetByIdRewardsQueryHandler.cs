using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Rewards;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Rewards.Querys.GetById
{
    internal sealed class GetByIdRewardsQueryHandler : IQueryHandler<GetByIdRewardsQuery, RewardsDTos>
    {
        private readonly IRewardsRepository _rewardsRepository;
        private readonly ILogger<GetByIdRewardsQueryHandler> _logger;

        public GetByIdRewardsQueryHandler(
            IRewardsRepository rewardsRepository, 
            ILogger<GetByIdRewardsQueryHandler> logger)
        {
            _rewardsRepository = rewardsRepository;
            _logger = logger;
        }

        public async Task<ResultT<RewardsDTos>> Handle(
            GetByIdRewardsQuery request, 
            CancellationToken cancellationToken)
        {
            var reward = await _rewardsRepository.GetByIdAsync(request.RewardsId);
            if (reward != null)
            {
                RewardsDTos rewardsDTos = new
                (
                    RewardsId: reward.Id,
                    Description: reward.Description,
                    PointAwarded: reward.PointAwarded,
                    DateAwarded: reward.DateAwarded
                );

                _logger.LogInformation("Reward with ID: {RewardsId} found.", request.RewardsId);

                return ResultT<RewardsDTos>.Success(rewardsDTos);
            }

            _logger.LogWarning("Reward with ID: {RewardsId} not found.", request.RewardsId);

            return ResultT<RewardsDTos>.Failure(Error.NotFound("400", $"Reward with ID {request.RewardsId} not found"));
        }
    }
}
