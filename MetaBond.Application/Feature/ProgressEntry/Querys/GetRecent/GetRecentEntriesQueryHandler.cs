using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressEntry.Querys.GetRecent
{
    internal sealed class GetRecentEntriesQueryHandler : IQueryHandler<GetRecentEntriesQuery, IEnumerable<ProgressEntryDTos>>
    {
        private readonly IProgressEntryRepository _repository;
        private readonly ILogger<GetRecentEntriesQueryHandler> _logger;

        public GetRecentEntriesQueryHandler(
            IProgressEntryRepository repository, 
            ILogger<GetRecentEntriesQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ResultT<IEnumerable<ProgressEntryDTos>>> Handle(
            GetRecentEntriesQuery request, 
            CancellationToken cancellationToken)
        {

            if (request != null)
            {
                if (request.TopCount <= 0)
                {
                    _logger.LogInformation("Invalid request: TopCount must be greater than zero.");
    
                    return ResultT<IEnumerable<ProgressEntryDTos>>.Failure(Error.Failure("400", "TopCount must be greater than zero."));
                }
                
                IEnumerable<Domain.Models.ProgressEntry> progressEntries = await _repository.GetRecentEntriesAsync(request.TopCount,cancellationToken);
                if (!progressEntries.Any())
                {
                    _logger.LogError("No recent progress entries found.");

                    return ResultT<IEnumerable<ProgressEntryDTos>>.Failure(Error.Failure("400", "No recent progress entries available."));
                }

                IEnumerable<ProgressEntryDTos> entryDTos = progressEntries.Select(x => new ProgressEntryDTos
                (
                    ProgressEntryId: x.Id,
                    ProgressBoardId: x.ProgressBoardId,
                    Description: x.Description,
                    CreatedAt: x.CreatedAt,
                    UpdateAt: x.UpdateAt
                ));

                _logger.LogInformation("Successfully retrieved {Count} recent progress entries.", entryDTos.Count());

                return ResultT<IEnumerable<ProgressEntryDTos>>.Success(entryDTos);
            }

            _logger.LogError("Invalid request: The request object is null.");

            return ResultT<IEnumerable<ProgressEntryDTos>>.Failure(Error.Failure("400", "Invalid request."));
        }
    }
}
