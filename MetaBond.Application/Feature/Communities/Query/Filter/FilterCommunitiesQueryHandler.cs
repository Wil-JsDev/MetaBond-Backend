using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communities;
using MetaBond.Application.Interfaces.Repository;
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
        public async Task<ResultT<IEnumerable<CommunitiesDTos>>> Handle(FilterCommunitiesQuery request, CancellationToken cancellationToken)
        {
            
            if (request.Category != null)
            {
                var exists = await communitiesRepository.ValidateAsync(x => x.Name == request.Category);
                if (!exists)
                {
                    logger.LogError("The specified category '{Category}' was not found.", request.Category);
                    return ResultT<IEnumerable<CommunitiesDTos>>.Failure(Error.NotFound("404", $"The category '{request.Category}' does not exist."));
                }
                
                var communitiesCategory = await decoratedCache.GetOrCreateAsync($"communitiesCategory-{request.Category}", async () =>
                {
                    return await communitiesRepository.GetByFilterAsync(x => x.Category == request.Category, cancellationToken);
                }, cancellationToken: cancellationToken);
                
                IEnumerable<Domain.Models.Communities> communitiesEnumerable = communitiesCategory.ToList();
                if (!communitiesEnumerable.Any())
                {
                    logger.LogError("No communities found for category '{Category}'.", request.Category);
                    return ResultT<IEnumerable<CommunitiesDTos>>.Failure(Error.Failure("400", "The list is empty"));
                }

                var dTos = communitiesEnumerable.Select(c => new CommunitiesDTos
                (
                    CommunitiesId: c.Id,
                   Name: c.Name,
                   Category: c.Category,
                   CreatedAt: c.CreateAt
                ));

                logger.LogInformation("Found {Count} communities for category '{Category}'.", communitiesEnumerable.Count(), request.Category);
                return ResultT<IEnumerable<CommunitiesDTos>>.Success(dTos);
            }

            logger.LogError("Filter request failed: No category provided.");
            return ResultT<IEnumerable<CommunitiesDTos>>.Failure(Error.Failure("404", $"{request.Category} not found"));

        }
    }

