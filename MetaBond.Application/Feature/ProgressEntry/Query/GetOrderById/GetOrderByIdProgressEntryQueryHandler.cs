using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressEntry.Querys.GetOrderById
{
    internal sealed class GetOrderByIdProgressEntryQueryHandler(
        IProgressEntryRepository progressEntryRepository,
        ILogger<GetOrderByIdProgressEntryQueryHandler> logger)
        : IQueryHandler<GetOrderByIdProgressEntryQuery, IEnumerable<ProgressEntryBasicDTos>>
    {
        public async Task<ResultT<IEnumerable<ProgressEntryBasicDTos>>> Handle(
            GetOrderByIdProgressEntryQuery request, 
            CancellationToken cancellationToken)
        {
            IEnumerable<Domain.Models.ProgressEntry> progressEntries = await progressEntryRepository.GetOrderByIdAsync(request.ProgressBoardId,cancellationToken);
            List<Domain.Models.ProgressEntry> enumerable = progressEntries.ToList();
            if (!enumerable.Any())
            {
                logger.LogError("No progress entries found when ordering by ID.");

                return ResultT<IEnumerable<ProgressEntryBasicDTos>>.Failure(Error.Failure("400", "No progress entries available."));
            }

            IEnumerable<ProgressEntryBasicDTos> entryBasicDTos = enumerable.Select(x => new ProgressEntryBasicDTos
            (
                ProgressEntryId: x.Id,
                Description: x.Description,
                ProgressBoardId: x.ProgressBoardId
            ));

            var progressEntryBasicDTosEnumerable = entryBasicDTos.ToList();
            logger.LogInformation("Successfully retrieved {Count} progress entries ordered by ID.", progressEntryBasicDTosEnumerable.Count());

            return ResultT<IEnumerable<ProgressEntryBasicDTos>>.Success(progressEntryBasicDTosEnumerable);
        }
    }
}
