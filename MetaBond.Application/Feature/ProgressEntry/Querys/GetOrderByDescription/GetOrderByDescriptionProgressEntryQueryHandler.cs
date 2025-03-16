
using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressEntry.Querys.GetOrderByDescription
{
    internal sealed class GetOrderByDescriptionProgressEntryQueryHandler(
        IProgressEntryRepository progressEntryRepository,
        ILogger<GetOrderByDescriptionProgressEntryQueryHandler> logger)
        : IQueryHandler<GetOrderByDescriptionProgressEntryQuery, IEnumerable<ProgressEntryWithDescriptionDTos>>
    {
        public async Task<ResultT<IEnumerable<ProgressEntryWithDescriptionDTos>>> Handle(
            GetOrderByDescriptionProgressEntryQuery request, 
            CancellationToken cancellationToken)
        {

            IEnumerable<Domain.Models.ProgressEntry> progressEntries = await progressEntryRepository.GetOrderByDescriptionAsync(request.ProgressBoardId,cancellationToken);
            if (progressEntries == null || !progressEntries.Any())
            {
                logger.LogError("No progress entries found when ordering by description.");

                return ResultT<IEnumerable<ProgressEntryWithDescriptionDTos>>.Failure(Error.Failure("400", "No progress entries found for the given description order."));
            }

            IEnumerable<ProgressEntryWithDescriptionDTos> descriptionDTos = progressEntries.Select(x => new ProgressEntryWithDescriptionDTos
            (
                Description: x.Description
            ));

            logger.LogInformation("Successfully retrieved {Count} progress entries ordered by description.", descriptionDTos.Count());

            return ResultT<IEnumerable<ProgressEntryWithDescriptionDTos>>.Success(descriptionDTos);
        }
    }
}
