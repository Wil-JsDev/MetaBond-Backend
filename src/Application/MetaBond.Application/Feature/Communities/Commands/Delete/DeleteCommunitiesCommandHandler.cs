using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Communities.Commands.Delete;

internal sealed class DeleteCommunitiesCommandHandler(
    ICommunitiesRepository communitiesRepository,
    ILogger<DeleteCommunitiesCommandHandler> logger)
    : ICommandHandler<DeleteCommunitiesCommand, Guid>
{
    public async Task<ResultT<Guid>> Handle(DeleteCommunitiesCommand request, CancellationToken cancellationToken)
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
            await communitiesRepository.DeleteAsync(communities.Value, cancellationToken);

            logger.LogInformation("Community with ID {CommunityId} successfully deleted.", request.Id);

            return ResultT<Guid>.Success(request.Id);
        }

        logger.LogError("Community with ID {CommunityId} not found for deletion.", request.Id);

        return ResultT<Guid>.Failure(Error.NotFound("404", $"{request.Id} not found"));
    }
}