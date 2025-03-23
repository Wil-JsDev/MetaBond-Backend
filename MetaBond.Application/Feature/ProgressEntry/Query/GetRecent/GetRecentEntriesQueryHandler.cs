using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressEntry.Query.GetRecent;

internal sealed class GetRecentEntriesQueryHandler(
    IProgressEntryRepository repository,
    ILogger<GetRecentEntriesQueryHandler> logger)
    : IQueryHandler<GetRecentEntriesQuery, IEnumerable<ProgressEntryDTos>>
{
    public async Task<ResultT<IEnumerable<ProgressEntryDTos>>> Handle(
        GetRecentEntriesQuery request, 
        CancellationToken cancellationToken)
    {
        if (request != null)
        {
            if (request.TopCount <= 0)
            {
                logger.LogInformation("Invalid request: TopCount must be greater than zero.");
    
                return ResultT<IEnumerable<ProgressEntryDTos>>.Failure(Error.Failure("400", "TopCount must be greater than zero."));
            }
                
            IEnumerable<Domain.Models.ProgressEntry> progressEntries = await repository.GetRecentEntriesAsync(request.ProgressBoardId,request.TopCount,cancellationToken);
            var enumerable = progressEntries.ToList();
            if (!enumerable.Any())
            {
                logger.LogError("No recent progress entries found.");

                return ResultT<IEnumerable<ProgressEntryDTos>>.Failure(Error.Failure("400", "No recent progress entries available."));
            }

            IEnumerable<ProgressEntryDTos> entryDTos = enumerable.Select(x => new ProgressEntryDTos
            (
                ProgressEntryId: x.Id,
                ProgressBoardId: x.ProgressBoardId,
                Description: x.Description,
                CreatedAt: x.CreatedAt,
                UpdateAt: x.UpdateAt
            ));

            var progressEntryDTosEnumerable = entryDTos.ToList();
            logger.LogInformation("Successfully retrieved {Count} recent progress entries.", progressEntryDTosEnumerable.Count());

            return ResultT<IEnumerable<ProgressEntryDTos>>.Success(progressEntryDTosEnumerable);
        }

        logger.LogError("Invalid request: The request object is null.");

        return ResultT<IEnumerable<ProgressEntryDTos>>.Failure(Error.Failure("400", "Invalid request."));
    }
}