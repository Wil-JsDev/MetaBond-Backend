using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Rewards;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Rewards.Querys.GetAll
{
    internal sealed class GetAllRewardsQueryHandler : IQueryHandler<GetAllRewardsQuery, IEnumerable<RewardsDTos>>
    {
        private readonly IRewardsRepository _rewardsRepository;
        private readonly ILogger<GetAllRewardsQueryHandler> _logger;

        public GetAllRewardsQueryHandler(
            IRewardsRepository rewardsRepository, 
            ILogger<GetAllRewardsQueryHandler> logger)
        {
            _rewardsRepository = rewardsRepository;
            _logger = logger;
        }

        public async Task<ResultT<IEnumerable<RewardsDTos>>> Handle(
            GetAllRewardsQuery request, 
            CancellationToken cancellationToken)
        {

            IEnumerable<Domain.Models.Rewards> rewardsList = await _rewardsRepository.GetAll(cancellationToken);
            if (!rewardsList.Any())
            {
                _logger.LogWarning("No rewards found in the repository");

                return ResultT<IEnumerable<RewardsDTos>>.Failure(Error.Failure("400","The List is empty"));
            }

            IEnumerable<RewardsDTos> rewardsDTos = rewardsList.Select(x => new RewardsDTos
            (
                RewardsId: x.Id,
                Description: x.Description,
                PointAwarded: x.PointAwarded,
                DateAwarded: x.DateAwarded
            ));

            _logger.LogInformation("Successfully retrieved {Count} rewards.", rewardsList.Count());

            return ResultT<IEnumerable<RewardsDTos>>.Success(rewardsDTos);
        }
    }
}
