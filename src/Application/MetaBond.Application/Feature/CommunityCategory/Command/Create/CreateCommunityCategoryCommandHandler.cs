using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.CommunityCategory;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.CommunityCategory.Command.Create;

internal sealed class CreateCommunityCategoryCommandHandler(
    ICommunityCategoryRepository communityCategoryRepository,
    ILogger<CreateCommunityCategoryCommandHandler> logger
) : ICommandHandler<CreateCommunityCategoryCommand, CommunityCategoryDTos>
{
    public async Task<ResultT<CommunityCategoryDTos>> Handle(CreateCommunityCategoryCommand request,
        CancellationToken cancellationToken)
    {
        if (await communityCategoryRepository.ExistsNameAsync(request.Name, cancellationToken))
        {
            logger.LogError("The name {RequestName} already exists.", request.Name);

            return ResultT<CommunityCategoryDTos>.Failure(
                Error.Failure("400", $"The name {request.Name} already exists."));
        }

        Domain.Models.CommunityCategory communityCategory = new()
        {
            Id = Guid.NewGuid(),
            Name = request.Name
        };

        await communityCategoryRepository.CreateAsync(communityCategory, cancellationToken);

        logger.LogInformation(
            "Community Category with name '{CommunityCategoryName}' was successfully created with ID {CommunityCategoryId}.",
            communityCategory.Name, communityCategory.Id);

        var communityCategoryDTos = CommunityCategoryMapper.MapCommunityCategoryDTos(communityCategory);

        return ResultT<CommunityCategoryDTos>.Success(communityCategoryDTos);
    }
}