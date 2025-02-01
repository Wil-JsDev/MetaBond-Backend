using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communties;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Communities.Querys.GetAll
{
    internal sealed class GetAllCommunitiesQueryHandler : IQueryHandler<GetlAllCommunitiesQuery,IEnumerable<CommunitiesDTos>>
    {
        private readonly ICommunitiesRepository _communitiesRepository;
        private readonly ILogger<GetAllCommunitiesQueryHandler> _logger;

        public GetAllCommunitiesQueryHandler(ICommunitiesRepository communitiesRepository, ILogger<GetAllCommunitiesQueryHandler> logger)
        {
            _communitiesRepository = communitiesRepository;
            _logger = logger;
        }

        public async Task<ResultT<IEnumerable<CommunitiesDTos>>> Handle(GetlAllCommunitiesQuery request, CancellationToken cancellationToken)
        {
            var communitiesAll = await _communitiesRepository.GetAll(cancellationToken);
            if (communitiesAll != null)
            {
                var communitiesDtosList = communitiesAll.Select(c => new CommunitiesDTos
                (
                    CommunitieId: c.Id,
                    Name: c.Name,
                    Category: c.Category,
                    CreatedAt: c.CreateAt
                ));

                if (!communitiesAll.Any())
                {
                    _logger.LogError("No communities found. The list is empty.");
                    return ResultT<IEnumerable<CommunitiesDTos>>.Failure(Error.Failure("400", "The list is empty"));
                }

                _logger.LogInformation("Successfully retrieved {Count} communities.", communitiesAll.Count());
                return ResultT<IEnumerable<CommunitiesDTos>>.Success(communitiesDtosList);
            }

            _logger.LogError("Failed to retrieve communities. The list is null.");
            return ResultT<IEnumerable<CommunitiesDTos>>.Failure(Error.Failure("400", "List of null"));

        }
    }
}
