using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Rewards.Query.GetCount;

internal sealed class GetCountRewardsQueryHandler(
    IRewardsRepository repository,
    IDistributedCache decoratedCache,
    ILogger<GetCountRewardsQueryHandler> logger)
    : IQueryHandler<GetCountRewardsQuery, int>
{
    public async Task<ResultT<int>> Handle(
        GetCountRewardsQuery request,
        CancellationToken cancellationToken)
    {
        if (request != null)
        {
            var rewardsCount = await decoratedCache.GetOrCreateAsync(
                "rewards-count",
                async () => await repository.CountRewardsAsync(cancellationToken), 
                cancellationToken: cancellationToken);
            
            logger.LogInformation("Total rewards counted: {Count}.", rewardsCount);

            return ResultT<int>.Success(rewardsCount);
        }

        logger.LogWarning("Failed to count rewards: The request was invalid or encountered an issue during processing.");

        return ResultT<int>.Failure(Error.NotFound("400", "Unable to count rewards due to an invalid request or processing error."));
    }

}