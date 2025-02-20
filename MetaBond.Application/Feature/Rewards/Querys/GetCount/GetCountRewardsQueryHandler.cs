using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Rewards.Querys.GetCount
{
    internal sealed class GetCountRewardsQueryHandler : IQueryHandler<GetCountRewardsQuery, int>
    {
        private readonly IRewardsRepository _repository;
        private readonly ILogger<GetCountRewardsQueryHandler> _logger;

        public GetCountRewardsQueryHandler(
            IRewardsRepository repository, 
            ILogger<GetCountRewardsQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ResultT<int>> Handle(
            GetCountRewardsQuery request, 
            CancellationToken cancellationToken)
        {
            
            var rewards = await _repository.GetByIdAsync(request.RewardsId);
            if (rewards != null)
            {
                var rewardsCount = await _repository.CountRewardsAsync(cancellationToken);

                _logger.LogInformation("Successfully counted {Count} rewards for ID: {RewardsId}.", rewardsCount, request.RewardsId);

                return ResultT<int>.Success(rewardsCount);
            }

            _logger.LogWarning("Reward with ID: {RewardsId} not found. Cannot count rewards.", request.RewardsId);

            return ResultT<int>.Failure(Error.NotFound("400",$"{request.RewardsId} not found"));
        }
    }
}
