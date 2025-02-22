using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Rewards.Commands.Delete
{
    internal sealed class DeleteRewardsCommandHandler : ICommandHandler<DeleteRewardsCommand, Guid>
    {
        private readonly IRewardsRepository _repository;
        private readonly ILogger<DeleteRewardsCommandHandler> _logger;

        public DeleteRewardsCommandHandler(
            IRewardsRepository repository, 
            ILogger<DeleteRewardsCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ResultT<Guid>> Handle(
            DeleteRewardsCommand request, 
            CancellationToken cancellationToken)
        {
            var reward = await _repository.GetByIdAsync(request.RewardsId);
            if (reward != null)
            {
                await _repository.DeleteAsync(reward,cancellationToken);

                _logger.LogInformation("Reward with ID {RewardsId} deleted successfully", reward.Id);

                return ResultT<Guid>.Success(reward.Id);
            }

            _logger.LogError("Reward with ID {RewardsId} not found", request.RewardsId);

            return ResultT<Guid>.Failure(Error.Failure("404", "Reward not found"));
        }
    }
}
