using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Rewards;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;
using Serilog.Core;

namespace MetaBond.Application.Feature.Rewards.Querys.GetTop
{
    internal sealed class GetTopRewardsQueryHandler : IQueryHandler<GetTopRewardsQuery, IEnumerable<RewardsDTos>>
    {
        private readonly IRewardsRepository _rewardsRepository;
        private readonly ILogger<GetTopRewardsQueryHandler> _logger;

        public GetTopRewardsQueryHandler(
            IRewardsRepository rewardsRepository, 
            ILogger<GetTopRewardsQueryHandler> logger)
        {
            _rewardsRepository = rewardsRepository;
            _logger = logger;
        }

        public async Task<ResultT<IEnumerable<RewardsDTos>>> Handle(
            GetTopRewardsQuery request, 
            CancellationToken cancellationToken)
        {

            if (request != null)
            {
                var rewardsList = await _rewardsRepository.GetTopRewardsByPointsAsync(request.TopCount,cancellationToken);
                if (rewardsList == null || !rewardsList.Any())
                {
                    _logger.LogError("No top rewards found");

                    return ResultT<IEnumerable<RewardsDTos>>.Failure(Error.Failure("400","The list is empty"));
                }

                IEnumerable<RewardsDTos> rewardsDTos = rewardsList.Select(x => new RewardsDTos
                (
                    RewardsId: x.Id,
                    Description: x.Description,
                    PointAwarded: x.PointAwarded,
                    DateAwarded:  x.DateAwarded
                ));

                _logger.LogInformation("Successfully retrieved {Count} top rewards.", rewardsList.Count());

                return ResultT<IEnumerable<RewardsDTos>>.Success(rewardsDTos);
            }
            _logger.LogError("Received a null request for fetching top rewards.");

            return ResultT<IEnumerable<RewardsDTos>>.Failure(Error.Failure("400","Invalid request"));
        }
    }
}
