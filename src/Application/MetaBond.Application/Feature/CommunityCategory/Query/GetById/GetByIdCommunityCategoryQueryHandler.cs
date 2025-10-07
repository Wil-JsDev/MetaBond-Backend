using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.CommunityCategory;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.CommunityCategory.Query.GetById;

internal sealed class GetByIdCommunityCategoryQueryHandler(
    ICommunityCategoryRepository communityCategoryRepository,
    ILogger<GetByIdCommunityCategoryQueryHandler> logger
) : IQueryHandler<GetByIdCommunityCategoryQuery, CommunityCategoryDTos>
{
    public async Task<ResultT<CommunityCategoryDTos>> Handle(GetByIdCommunityCategoryQuery request,
        CancellationToken cancellationToken)
    {
        var communityCategory = await EntityHelper.GetEntityByIdAsync(
            communityCategoryRepository.GetByIdAsync,
            request.CommunityCategoryId,
            "Community Category",
            logger
        );

        if (!communityCategory.IsSuccess)
            return ResultT<CommunityCategoryDTos>.Failure(communityCategory.Error!);

        logger.LogInformation("Community Category with ID {CommunityCategoryId} was successfully retrieved.",
            request.CommunityCategoryId);

        return ResultT<CommunityCategoryDTos>.Success(
            CommunityCategoryMapper.MapCommunityCategoryDTos(communityCategory.Value)
        );
    }
}