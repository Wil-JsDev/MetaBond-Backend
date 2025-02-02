using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Communities.Commands.Delete
{
    internal sealed class DeleteCommunitiesCommandHandler : ICommandHandler<DeleteCommunitiesCommand, Guid>
    {

        private readonly ICommunitiesRepository _communitiesRepository;
        private readonly ILogger<DeleteCommunitiesCommandHandler> _logger;

        public DeleteCommunitiesCommandHandler(ICommunitiesRepository communitiesRepository, ILogger<DeleteCommunitiesCommandHandler> logger)
        {
            _communitiesRepository = communitiesRepository;
            _logger = logger;
        }

        public async Task<ResultT<Guid>> Handle(DeleteCommunitiesCommand request, CancellationToken cancellationToken)
        {
            var communities = await _communitiesRepository.GetByIdAsync(request.Id);
            if (communities != null)
            {
                await _communitiesRepository.DeleteAsync(communities,cancellationToken);
                _logger.LogInformation("Community with ID {CommunityId} successfully deleted.", request.Id);

                return ResultT<Guid>.Success(request.Id);
            }

            _logger.LogError("Community with ID {CommunityId} not found for deletion.", request.Id);
            return ResultT<Guid>.Failure(Error.NotFound("404", $"{request.Id} not found"));

        }
    }
}
