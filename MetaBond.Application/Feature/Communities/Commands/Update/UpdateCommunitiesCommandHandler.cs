using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communties;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Communities.Commands.Update;

internal sealed class UpdateCommunitiesCommandHandler(
    ICommunitiesRepository communitiesRepository,
    ILogger<UpdateCommunitiesCommandHandler> logger)
    : ICommandHandler<UpdateCommunitiesCommand, CommunitiesDTos>
{
    public async Task<ResultT<CommunitiesDTos>> Handle(UpdateCommunitiesCommand request, CancellationToken cancellationToken)
    {
        var communities = await communitiesRepository.GetByIdAsync(request.Id);

        if (communities != null)
        {
            communities.Name = request.Name;
            communities.Category = request.Category;

            await communitiesRepository.UpdateAsync(communities, cancellationToken);

            logger.LogInformation("Community with ID {CommunityId} successfully updated.", request.Id);

            CommunitiesDTos communitiesDTos = new(
                CommunitieId: communities.Id,
                Name: communities.Name,
                Category: communities.Category,
                CreatedAt: communities.CreateAt
            );

            return ResultT<CommunitiesDTos>.Success(communitiesDTos);
        }
        logger.LogError("Community with ID {CommunityId} not found for update.", request.Id);
        
        return ResultT<CommunitiesDTos>.Failure(Error.Failure("404", $"{request.Id} not found"));
    }
}