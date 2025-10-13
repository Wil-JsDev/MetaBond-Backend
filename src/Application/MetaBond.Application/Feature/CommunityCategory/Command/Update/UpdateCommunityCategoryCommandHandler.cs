using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.CommunityCategory;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.CommunityCategory.Command.Update;

internal sealed class UpdateCommunityCategoryCommandHandler(
    ILogger<UpdateCommunityCategoryCommandHandler> logger,
    ICommunityCategoryRepository communityCategoryRepository
) : ICommandHandler<UpdateCommunityCategoryCommand, CommunityCategoryDTos>
{
    public async Task<ResultT<CommunityCategoryDTos>> Handle(UpdateCommunityCategoryCommand request,
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

        if (await communityCategoryRepository.ExistsNameExceptIdAsync(request.Name!, request.CommunityCategoryId,
                cancellationToken))
        {
            logger.LogError("The name '{RequestName}' already exists in another Community Category.", request.Name);

            return ResultT<CommunityCategoryDTos>.Failure(
                Error.Failure("400", $"The name '{request.Name}' already exists."));
        }

        var oldName = communityCategory.Value.Name;
        communityCategory.Value.Name = request.Name;
        communityCategory.Value.UpdateAt = DateTime.UtcNow;

        await communityCategoryRepository.UpdateAsync(communityCategory.Value, cancellationToken);

        logger.LogInformation(
            "Community Category updated successfully. Id: {CommunityCategoryId}, Old Name: '{OldName}', New Name: '{NewName}'",
            communityCategory.Value.Id,
            oldName,
            communityCategory.Value.Name
        );

        return ResultT<CommunityCategoryDTos>.Success(
            CommunityCategoryMapper.MapCommunityCategoryDTos(communityCategory.Value)
        );
    }
}