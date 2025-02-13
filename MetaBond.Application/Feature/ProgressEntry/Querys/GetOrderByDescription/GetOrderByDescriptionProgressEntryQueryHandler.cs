
using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressEntry.Querys.GetOrderByDescription
{
    internal sealed class GetOrderByDescriptionProgressEntryQueryHandler : IQueryHandler<GetOrderByDescriptionProgressEntryQuery, IEnumerable<ProgressEntryWithDescriptionDTos>>
    {
        private readonly IProgressEntryRepository _progressEntryRepository;
        private readonly ILogger<GetOrderByDescriptionProgressEntryQueryHandler> _logger;

        public GetOrderByDescriptionProgressEntryQueryHandler(
            IProgressEntryRepository progressEntryRepository, 
            ILogger<GetOrderByDescriptionProgressEntryQueryHandler> logger)
        {
            _progressEntryRepository = progressEntryRepository;
            _logger = logger;
        }

        public async Task<ResultT<IEnumerable<ProgressEntryWithDescriptionDTos>>> Handle(
            GetOrderByDescriptionProgressEntryQuery request, 
            CancellationToken cancellationToken)
        {

            IEnumerable<Domain.Models.ProgressEntry> progressEntries = await _progressEntryRepository.GetOrderByDescriptionAsync(cancellationToken);
            if (progressEntries == null || !progressEntries.Any())
            {
                _logger.LogError("No progress entries found when ordering by description.");

                return ResultT<IEnumerable<ProgressEntryWithDescriptionDTos>>.Failure(Error.Failure("400", "No progress entries found for the given description order."));
            }

            IEnumerable<ProgressEntryWithDescriptionDTos> descriptionDTos = progressEntries.Select(x => new ProgressEntryWithDescriptionDTos
            (
                Description: x.Description
            ));

            _logger.LogInformation("Successfully retrieved {Count} progress entries ordered by description.", descriptionDTos.Count());

            return ResultT<IEnumerable<ProgressEntryWithDescriptionDTos>>.Success(descriptionDTos);
        }
    }
}
