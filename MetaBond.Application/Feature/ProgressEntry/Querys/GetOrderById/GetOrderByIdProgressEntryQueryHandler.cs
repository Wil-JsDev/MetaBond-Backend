using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressEntry.Querys.GetOrderById
{
    internal sealed class GetOrderByIdProgressEntryQueryHandler : IQueryHandler<GetOrderByIdProgressEntryQuery, IEnumerable<ProgressEntryBasicDTos>>
    {
        private readonly IProgressEntryRepository _progressEntryRepository;
        private readonly ILogger<GetOrderByIdProgressEntryQueryHandler> _logger;

        public GetOrderByIdProgressEntryQueryHandler(
            IProgressEntryRepository progressEntryRepository, 
            ILogger<GetOrderByIdProgressEntryQueryHandler> logger)
        {
            _progressEntryRepository = progressEntryRepository;
            _logger = logger;
        }

        public async Task<ResultT<IEnumerable<ProgressEntryBasicDTos>>> Handle(
            GetOrderByIdProgressEntryQuery request, 
            CancellationToken cancellationToken)
        {
            IEnumerable<Domain.Models.ProgressEntry> progressEntries = await _progressEntryRepository.GetOrderByIdAsync(cancellationToken);
            if (!progressEntries.Any())
            {
                _logger.LogError("No progress entries found when ordering by ID.");

                return ResultT<IEnumerable<ProgressEntryBasicDTos>>.Failure(Error.Failure("400", "No progress entries available."));
            }

            IEnumerable<ProgressEntryBasicDTos> entryBasicDTos = progressEntries.Select(x => new ProgressEntryBasicDTos
            (
                ProgressEntryId: x.Id,
                Description: x.Description,
                ProgressBoardId: x.ProgressBoardId
            ));

            _logger.LogInformation("Successfully retrieved {Count} progress entries ordered by ID.", entryBasicDTos.Count());

            return ResultT<IEnumerable<ProgressEntryBasicDTos>>.Success(entryBasicDTos);
        }
    }
}
