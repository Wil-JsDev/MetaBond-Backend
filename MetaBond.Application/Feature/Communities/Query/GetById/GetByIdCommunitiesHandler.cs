using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communties;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Communities.Query.GetById;

    public class GetByIdCommunitiesHandler(
        ICommunitiesRepository communitiesRepository,
        IDistributedCache decoratedCache,
        ILogger<GetByIdCommunitiesHandler> logger)
        : IQueryHandler<GetByIdCommunitiesQuery, CommunitiesDTos>
    {
        public async Task<ResultT<CommunitiesDTos>> Handle(GetByIdCommunitiesQuery request, CancellationToken cancellationToken)
        {
            var communities = await decoratedCache.GetOrCreateAsync($"communities-{request.Id}", async () => await communitiesRepository.GetByIdAsync(request.Id), cancellationToken: cancellationToken);
            if (communities != null)
            {
                CommunitiesDTos dTos = new
                (
                    CommunitieId: communities.Id,
                    Name: communities.Name,
                    Category: communities.Category,
                    CreatedAt: communities.CreateAt
                );

                logger.LogInformation("Successfully retrieved community with ID {CommunityId}.", request.Id);
                return ResultT<CommunitiesDTos>.Success(dTos);

            }

            logger.LogError("Community with ID {CommunityId} was not found.", request.Id);
            return ResultT<CommunitiesDTos>.Failure(Error.NotFound("404", $"Community with ID {request.Id} was not found."));

        }
    }
