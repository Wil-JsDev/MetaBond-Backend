using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Rewards;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Rewards.Commands.Create
{
    internal sealed class CreateRewardsCommandHandler : ICommandHandler<CreateRewardsCommand, RewardsDTos>
    {
        private readonly IRewardsRepository _rewardsRepository;
        private readonly ILogger<CreateRewardsCommandHandler> _logger;

        public CreateRewardsCommandHandler(
            IRewardsRepository rewardsRepository, 
            ILogger<CreateRewardsCommandHandler> logger)
        {
            _rewardsRepository = rewardsRepository;
            _logger = logger;
        }

        public async Task<ResultT<RewardsDTos>> Handle(
            CreateRewardsCommand request, 
            CancellationToken cancellationToken)
        {

            if (request != null)
            {
                Domain.Models.Rewards rewardsModel = new()
                {
                    Id = Guid.NewGuid(),
                    Description = request.Description,
                    PointAwarded = request.PointAwarded,
                    DateAwarded = DateTime.UtcNow
                };

                await _rewardsRepository.CreateAsync(rewardsModel, cancellationToken);

                _logger.LogInformation("Reward created successfully with ID: {RewardsId}", rewardsModel.Id);

                RewardsDTos rewardsDTos = new
                (
                    RewardsId: rewardsModel.Id,
                    Description: rewardsModel.Description,
                    PointAwarded: rewardsModel.PointAwarded,
                    DateAwarded: rewardsModel.DateAwarded
                );

                return ResultT<RewardsDTos>.Success(rewardsDTos);
            }

            _logger.LogError("Failed to create reward: invalid request data");

            return ResultT<RewardsDTos>.Failure(Error.Failure("400","Invalid request"));
        }
    }
}
