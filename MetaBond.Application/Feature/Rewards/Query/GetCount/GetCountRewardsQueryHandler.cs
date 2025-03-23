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
            if (request != null)
            {
                var rewardsCount = await _repository.CountRewardsAsync(cancellationToken);

                _logger.LogInformation("Total rewards counted: {Count}.", rewardsCount);

                return ResultT<int>.Success(rewardsCount);
            }

            _logger.LogWarning("Failed to count rewards: The request was invalid or encountered an issue during processing.");

            return ResultT<int>.Failure(Error.NotFound("400", "Unable to count rewards due to an invalid request or processing error."));
        }

    }
}
