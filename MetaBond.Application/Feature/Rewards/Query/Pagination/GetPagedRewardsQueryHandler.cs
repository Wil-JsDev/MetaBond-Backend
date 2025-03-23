using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Rewards;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Rewards.Querys.Pagination
{
    internal sealed class GetPagedRewardsQueryHandler : IQueryHandler<GetPagedRewardsQuery, PagedResult<RewardsDTos>>
    {
        private readonly IRewardsRepository _rewardsRepository;
        private readonly ILogger<GetPagedRewardsQueryHandler> _logger;

        public GetPagedRewardsQueryHandler(
            IRewardsRepository rewardsRepository, 
            ILogger<GetPagedRewardsQueryHandler> logger)
        {
            _rewardsRepository = rewardsRepository;
            _logger = logger;
        }

        public async Task<ResultT<PagedResult<RewardsDTos>>> Handle(
            GetPagedRewardsQuery request, 
            CancellationToken cancellationToken)
        {
            if (request != null)
            {
                var pagedRewards = await _rewardsRepository.GetPagedRewardsAsync(
                    request.PageNmber,
                    request.PageSize,
                    cancellationToken);

                var rewardsDto = pagedRewards.Items.Select(x => new RewardsDTos
                (
                    RewardsId: x.Id,
                    Description: x.Description,
                    PointAwarded: x.PointAwarded,
                    DateAwarded: x.DateAwarded
                ));

                if (!rewardsDto.Any())
                {
                    _logger.LogWarning("No rewards found for the requested page {PageNumber}.", request.PageNmber);

                    return ResultT<PagedResult<RewardsDTos>>.Failure(Error.Failure("400", "No rewards available for the requested page."));
                }


                PagedResult<RewardsDTos> result = new()
                {
                    TotalItems = pagedRewards.TotalItems,
                    TotalPages = pagedRewards.TotalPages,
                    CurrentPage = pagedRewards.CurrentPage,
                    Items = rewardsDto
                };

                _logger.LogInformation("Successfully retrieved {Count} rewards for page {PageNumber}.", rewardsDto.Count(), request.PageNmber);
                
                return ResultT<PagedResult<RewardsDTos>>.Success(result);
            }

            _logger.LogError("Received a null request for paginated rewards.");

            return ResultT<PagedResult<RewardsDTos>>.Failure(Error.Failure("400", "Invalid request."));
        }
    }
}
