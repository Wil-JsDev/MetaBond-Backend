using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communities;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using MetaBond.Domain.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Communities.Query.GetById;

public class GetByIdCommunitiesHandler(
    ICommunitiesRepository communitiesRepository,
    ILogger<GetByIdCommunitiesHandler> logger)
    : IQueryHandler<GetByIdCommunitiesQuery, CommunitiesDTos>
{
    public async Task<ResultT<CommunitiesDTos>> Handle(GetByIdCommunitiesQuery request,
        CancellationToken cancellationToken)
    {
        var communities = await EntityHelper.GetEntityByIdAsync
        (
            communitiesRepository.GetByIdAsync,
            request.Id,
            "Communities",
            logger
        );

        if (!communities.IsSuccess)
            return communities.Error!;

        var communitiesDto = CommunityMapper.MapCommunitiesDTos(communities.Value);

        logger.LogInformation("Successfully retrieved community with ID {CommunityId}.", request.Id);

        return ResultT<CommunitiesDTos>.Success(communitiesDto);
    }
}