using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communties;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Communities.Querys.GetById
{
    public class GetByIdCommunitiesHandler : IQueryHandler<GetByIdCommunitiesQuery, CommunitiesDTos>
    {

        private readonly ICommunitiesRepository _communitiesRepository;
        private readonly ILogger<GetByIdCommunitiesHandler> _logger;

        public GetByIdCommunitiesHandler(ICommunitiesRepository communitiesRepository, ILogger<GetByIdCommunitiesHandler> logger)
        {
            _communitiesRepository = communitiesRepository;
            _logger = logger;
        }

        public async Task<ResultT<CommunitiesDTos>> Handle(GetByIdCommunitiesQuery request, CancellationToken cancellationToken)
        {
            var communities = await _communitiesRepository.GetByIdAsync(request.Id);
            if (communities != null)
            {
                CommunitiesDTos dTos = new
                (
                    CommunitieId: communities.Id,
                    Name: communities.Name,
                    Category: communities.Category,
                    CreatedAt: communities.CreateAt
                );

                _logger.LogInformation("Successfully retrieved community with ID {CommunityId}.", request.Id);
                return ResultT<CommunitiesDTos>.Success(dTos);

            }

            _logger.LogError("Community with ID {CommunityId} was not found.", request.Id);
            return ResultT<CommunitiesDTos>.Failure(Error.NotFound("404", $"Community with ID {request.Id} was not found."));

        }
    }
}
