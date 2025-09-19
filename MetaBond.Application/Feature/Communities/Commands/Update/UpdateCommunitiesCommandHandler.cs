using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communities;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Communities.Commands.Update;

internal sealed class UpdateCommunitiesCommandHandler(
    ICommunitiesRepository communitiesRepository,
    ILogger<UpdateCommunitiesCommandHandler> logger)
    : ICommandHandler<UpdateCommunitiesCommand, CommunitiesDTos>
{
    public async Task<ResultT<CommunitiesDTos>> Handle(UpdateCommunitiesCommand request,
        CancellationToken cancellationToken)
    {
        var communities = await EntityHelper.GetEntityByIdAsync
        (
            communitiesRepository.GetByIdAsync,
            request.Id,
            "Communities",
            logger
        );

        if (communities.IsSuccess)
        {
            communities.Value.Name = request.Name;
            communities.Value.Description = request.Description;

            await communitiesRepository.UpdateAsync(communities.Value, cancellationToken);

            logger.LogInformation("Community with ID {CommunityId} successfully updated.", request.Id);

            var communitiesDTos = CommunityMapper.MapCommunitiesDTos(communities.Value);

            return ResultT<CommunitiesDTos>.Success(communitiesDTos);
        }

        logger.LogError("Community with ID {CommunityId} not found for update.", request.Id);

        return ResultT<CommunitiesDTos>.Failure(Error.Failure("404", $"{request.Id} not found"));
    }
}