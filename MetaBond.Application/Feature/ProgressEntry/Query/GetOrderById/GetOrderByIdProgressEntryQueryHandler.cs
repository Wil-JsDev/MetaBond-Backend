using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressEntry.Query.GetOrderById;

internal sealed class GetOrderByIdProgressEntryQueryHandler(
    IProgressEntryRepository progressEntryRepository,
    IDistributedCache decoratedCache,
    ILogger<GetOrderByIdProgressEntryQueryHandler> logger)
    : IQueryHandler<GetOrderByIdProgressEntryQuery, IEnumerable<ProgressEntryBasicDTos>>
{
    public async Task<ResultT<IEnumerable<ProgressEntryBasicDTos>>> Handle(
        GetOrderByIdProgressEntryQuery request, 
        CancellationToken cancellationToken)
    {

        var progressEntries = await decoratedCache.GetOrCreateAsync(
            $"order-by-id-progress-board-{request.ProgressBoardId}",
            async () => await progressEntryRepository.GetOrderByIdAsync(request.ProgressBoardId, cancellationToken), 
            cancellationToken: cancellationToken);
        
        var enumerable = progressEntries.ToList();
        if (!enumerable.Any())
        {
            logger.LogError("No progress entries found when ordering by ID.");

            return ResultT<IEnumerable<ProgressEntryBasicDTos>>.Failure(Error.Failure("400", "No progress entries available."));
        }

        IEnumerable<ProgressEntryBasicDTos> entryBasicDTos = enumerable.Select(x => new ProgressEntryBasicDTos
        (
            ProgressEntryId: x.Id,
            Description: x.Description,
            ProgressBoardId: x.ProgressBoardId,
            UserId: x.UserId
        ));

        var progressEntryBasicDTosEnumerable = entryBasicDTos.ToList();
        
        logger.LogInformation("Successfully retrieved {Count} progress entries ordered by ID.", progressEntryBasicDTosEnumerable.Count());

        return ResultT<IEnumerable<ProgressEntryBasicDTos>>.Success(progressEntryBasicDTosEnumerable);
    }
}