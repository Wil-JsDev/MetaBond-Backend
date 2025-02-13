using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Rewards;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Rewards.Commands.Update
{
    internal sealed class UpdateRewardsCommandHandler : ICommandHandler<UpdateRewardsCommand, RewardsDTos>
    {
        private readonly IRewardsRepository _rewardsRepository;
        private readonly ILogger<UpdateRewardsCommandHandler> _logger;

        public UpdateRewardsCommandHandler(
            IRewardsRepository rewardsRepository, 
            ILogger<UpdateRewardsCommandHandler> logger)
        {
            _rewardsRepository = rewardsRepository;
            _logger = logger;
        }

        public async Task<ResultT<RewardsDTos>> Handle(
            UpdateRewardsCommand request, 
            CancellationToken cancellationToken)
        {
            var reward = await _rewardsRepository.GetByIdAsync(request.RewardsId);
            if (reward != null)
            {
                reward.Description = request.Description;

                reward.PointAwarded = request.PointAwarded;

                await _rewardsRepository.UpdateAsync(reward,cancellationToken);

                _logger.LogInformation("Reward with ID: {RewardsId} updated successfully.", request.RewardsId);

                RewardsDTos rewardsDTos = new
                (
                    RewardsId: reward.Id,
                    Description: reward.Description,
                    PointAwarded: reward.PointAwarded,
                    DateAwarded: reward.DateAwarded
                );

                return ResultT<RewardsDTos>.Success(rewardsDTos);
            }

            _logger.LogError("No reward found with ID: {RewardsId}", request.RewardsId);

            return ResultT<RewardsDTos>.Failure(Error.NotFound("404", $"{request.RewardsId} not found"));
        }
    }
}
