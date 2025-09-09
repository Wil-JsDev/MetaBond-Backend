using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.CommunityCategory;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.CommunityCategory.Query.GetByName;

internal sealed class GetByNameCommunityCategoryQueryHandler(
    ILogger<GetByNameCommunityCategoryQueryHandler> logger,
    ICommunityCategoryRepository communityCategoryRepository
) : IQueryHandler<GetByNameCommunityCategoryQuery, CommunityCategoryDTos>
{
    public async Task<ResultT<CommunityCategoryDTos>> Handle(GetByNameCommunityCategoryQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            logger.LogWarning("Community Category name is null or empty.");

            return ResultT<CommunityCategoryDTos>.Failure(Error.Failure("400", "Community Category name is required."));
        }

        var communityCategory = await communityCategoryRepository.GetByNameAsync(request.Name, cancellationToken);

        if (communityCategory is null)
        {
            logger.LogWarning("Community Category with name '{CommunityCategoryName}' not found.", request.Name);

            return ResultT<CommunityCategoryDTos>.Failure(Error.NotFound("404", "Community Category not found"));
        }

        var communityCategoryDto = CommunityCategoryMapper.MapCommunityCategoryDTos(communityCategory);

        logger.LogInformation("Community Category '{CommunityCategoryName}' retrieved successfully.", request.Name);

        return ResultT<CommunityCategoryDTos>.Success(communityCategoryDto);
    }
}