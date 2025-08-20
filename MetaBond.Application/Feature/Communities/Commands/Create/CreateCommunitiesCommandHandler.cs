using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communities;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Communities.Commands.Create;

internal sealed class CreateCommunitiesCommandHandler(
    ICommunitiesRepository communitiesRepository,
    ILogger<CreateCommunitiesCommandHandler> logger)
    : ICommandHandler<CreateCommunitiesCommand, CommunitiesDTos>
{
    public async Task<ResultT<CommunitiesDTos>> Handle(
        CreateCommunitiesCommand request,
        CancellationToken cancellationToken)
    {
            var exists = await communitiesRepository.ValidateAsync(x => x.Name == request.Name);
            if (exists)
            {
                logger.LogError($"The name {request.Name} already exists.");

                return ResultT<CommunitiesDTos>.Failure(
                    Error.Failure("400", $"The name {request.Name} already exists."));
            }

            Domain.Models.Communities communities = new()
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                Category = request.Category
            };

            await communitiesRepository.CreateAsync(communities, cancellationToken);
            
            logger.LogInformation("Community {CommunityId} created successfully.", communities.Id);

            var communitiesDTos = CommunityMapper.MapCommunitiesDTos(communities);

            return ResultT<CommunitiesDTos>.Success(communitiesDTos);
    }
}