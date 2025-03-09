using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communties;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Communities.Querys.Filter
{
    internal sealed class FilterCommunitiesQueryHandler : IQueryHandler<FilterCommunitiesQuery,IEnumerable<CommunitiesDTos>>
    {
        private readonly ICommunitiesRepository _communitiesRepository;
        private readonly ILogger<FilterCommunitiesQueryHandler> _logger;

        public FilterCommunitiesQueryHandler(ICommunitiesRepository communitiesRepository, ILogger<FilterCommunitiesQueryHandler> logger)
        {
            _communitiesRepository = communitiesRepository;
            _logger = logger;
        }

        public async Task<ResultT<IEnumerable<CommunitiesDTos>>> Handle(FilterCommunitiesQuery request, CancellationToken cancellationToken)
        {
            
            if (request.Category != null)
            {
                var exists = await _communitiesRepository.ValidateAsync(x => x.Name == request.Category);
                if (!exists)
                {
                    _logger.LogError("The specified category '{Category}' was not found.", request.Category);
                    return ResultT<IEnumerable<CommunitiesDTos>>.Failure(Error.NotFound("404", $"The category '{request.Category}' does not exist."));
                }

                var communitiesByCatagory = await _communitiesRepository.GetByFilterAsync(x => x.Category == request.Category,cancellationToken);
                if (!communitiesByCatagory.Any())
                {
                    _logger.LogError("No communities found for category '{Category}'.", request.Category);
                    return ResultT<IEnumerable<CommunitiesDTos>>.Failure(Error.Failure("400", "The list is empty"));
                }

                var dTos = communitiesByCatagory.Select(c => new CommunitiesDTos
                (
                   CommunitieId: c.Id,
                   Name: c.Name,
                   Category: c.Category,
                   CreatedAt: c.CreateAt
                ));

                _logger.LogInformation("Found {Count} communities for category '{Category}'.", communitiesByCatagory.Count(), request.Category);
                return ResultT<IEnumerable<CommunitiesDTos>>.Success(dTos);
            }

            _logger.LogError("Filter request failed: No category provided.");
            return ResultT<IEnumerable<CommunitiesDTos>>.Failure(Error.Failure("404", $"{request.Category} not found"));

        }
    }
}
