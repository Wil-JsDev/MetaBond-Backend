using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communities;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Interfaces.Service;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Communities.Commands.Create;

internal sealed class CreateCommunitiesCommandHandler(
    ICommunitiesRepository communitiesRepository,
    ICommunityCategoryRepository communityCategoryRepository,
    ILogger<CreateCommunitiesCommandHandler> logger,
    ICloudinaryService cloudinaryService
)
    : ICommandHandler<CreateCommunitiesCommand, CommunitiesDTos>
{
    public async Task<ResultT<CommunitiesDTos>> Handle(
        CreateCommunitiesCommand request,
        CancellationToken cancellationToken)
    {
        var exists = await communitiesRepository.ValidateAsync(x => x.Name == request.Name);
        if (exists)
        {
            logger.LogError("The name {RequestName} already exists.", request.Name);

            return ResultT<CommunitiesDTos>.Failure(
                Error.Failure("400", $"The name {request.Name} already exists."));
        }

        var communityCategory = await EntityHelper.GetEntityByIdAsync(
            communityCategoryRepository.GetByIdAsync,
            request.CategoryId ?? Guid.Empty,
            "Community Category",
            logger
        );

        if (!communityCategory.IsSuccess) return ResultT<CommunitiesDTos>.Failure(communityCategory.Error!);

        string imageUrl = "";
        if (request.ImageFile is not null)
        {
            await using var stream = request.ImageFile.OpenReadStream();
            imageUrl = await cloudinaryService.UploadImageCloudinaryAsync(
                stream,
                request.ImageFile.FileName,
                cancellationToken);
        }

        Domain.Models.Communities communities = new()
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            CommunityCategoryId = request.CategoryId,
            Photo = imageUrl
        };

        await communitiesRepository.CreateAsync(communities, cancellationToken);

        logger.LogInformation("Community {CommunityId} created successfully.", communities.Id);

        var communitiesDTos = CommunityMapper.MapCommunitiesDTos(communities);

        return ResultT<CommunitiesDTos>.Success(communitiesDTos);
    }
}