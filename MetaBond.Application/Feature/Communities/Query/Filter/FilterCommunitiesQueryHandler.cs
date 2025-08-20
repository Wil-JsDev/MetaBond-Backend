using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communities;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Communities.Query.Filter;

internal sealed class FilterCommunitiesQueryHandler(
    ICommunitiesRepository communitiesRepository,
    IDistributedCache decoratedCache,
    ILogger<FilterCommunitiesQueryHandler> logger)
    : IQueryHandler<FilterCommunitiesQuery, IEnumerable<CommunitiesDTos>>
{
    public async Task<ResultT<IEnumerable<CommunitiesDTos>>> Handle(FilterCommunitiesQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Category))
        {
            logger.LogError("The provided category is null or empty.");

            return ResultT<IEnumerable<CommunitiesDTos>>.Failure(Error.Failure("400",
                "The category cannot be null or empty"));
        }

        var exists = await communitiesRepository.ValidateAsync(x => x.Name == request.Category);
        if (!exists)
        {
            logger.LogError("The specified category '{Category}' was not found.", request.Category);
            return ResultT<IEnumerable<CommunitiesDTos>>.Failure(Error.NotFound("404",
                $"The category '{request.Category}' does not exist."));
        }

        var communitiesCategoryDto = await decoratedCache.GetOrCreateAsync($"communitiesCategory-{request.Category}",
            async () =>
            {
                var communities =
                    await communitiesRepository.GetByFilterAsync(x => x.Category == request.Category,
                        cancellationToken);
                var dTos = communities.Select(CommunityMapper.MapCommunitiesDTos);

                return dTos;
                
            }, cancellationToken: cancellationToken);

        IEnumerable<CommunitiesDTos> dTosEnumerable = communitiesCategoryDto.ToList();
        if (!dTosEnumerable.Any())
        {
            logger.LogError("No communities found for category '{Category}'.", request.Category);
            return ResultT<IEnumerable<CommunitiesDTos>>.Failure(Error.Failure("400", "The list is empty"));
        }

        logger.LogInformation("Found {Count} communities for category '{Category}'.", dTosEnumerable.Count(),
            request.Category);

        return ResultT<IEnumerable<CommunitiesDTos>>.Success(dTosEnumerable);
    }
}