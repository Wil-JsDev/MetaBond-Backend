using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communties;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Communities.Commands.Update
{
    internal sealed class UpdateCommunitiesCommandHandler : ICommandHandler<UpdateCommunitiesCommand, CommunitiesDTos>
    {
        private readonly ICommunitiesRepository _communitiesRepository;
        private readonly ILogger<UpdateCommunitiesCommandHandler> _logger;
        public UpdateCommunitiesCommandHandler(ICommunitiesRepository communitiesRepository, ILogger<UpdateCommunitiesCommandHandler> logger)
        {
            _communitiesRepository = communitiesRepository;
            _logger = logger;
        }

        public async Task<ResultT<CommunitiesDTos>> Handle(UpdateCommunitiesCommand request, CancellationToken cancellationToken)
        {
            var communites = await _communitiesRepository.GetByIdAsync(request.Id);

            if (communites != null)
            {
                communites.Name = request.Name;
                communites.Category = request.Category;

                await _communitiesRepository.UpdateAsync(communites, cancellationToken);

                _logger.LogInformation("Community with ID {CommunityId} successfully updated.", request.Id);

                CommunitiesDTos communitiesDTos = new(
                    CommunitieId: communites.Id,
                    Name: communites.Name,
                    Category: communites.Category,
                    CreatedAt: communites.CreateAt
                );

                return ResultT<CommunitiesDTos>.Success(communitiesDTos);
            }

            _logger.LogError("Community with ID {CommunityId} not found for update.", request.Id);
            return ResultT<CommunitiesDTos>.Failure(Error.Failure("404", $"{request.Id} not found"));
            
        }
    }
}
