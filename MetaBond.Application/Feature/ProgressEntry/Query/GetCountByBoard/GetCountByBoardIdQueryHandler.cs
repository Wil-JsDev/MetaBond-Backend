using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressEntry.Query.GetCountByBoard;

internal sealed class GetCountByBoardIdQueryHandler(
    IProgressEntryRepository progressEntryRepository,
    IDistributedCache decoratedCache,
    ILogger<GetCountByBoardIdQueryHandler> logger)
    : IQueryHandler<GetCountByBoardIdQuery, int>
{
    public async Task<ResultT<int>> Handle(
        GetCountByBoardIdQuery request, 
        CancellationToken cancellationToken)
    {

        if (request != null)
        {
            var countBoard = await decoratedCache.GetOrCreateAsync(
                $"count-entries-by-board-{request.ProgressBoardId}",
                async () => await progressEntryRepository.CountEntriesByBoardIdAsync(request.ProgressBoardId, cancellationToken), 
                cancellationToken: cancellationToken);
            
            logger.LogInformation("Successfully retrieved the count of progress entries for ProgressBoardId: {BoardId}. Count: {Count}", 
                request.ProgressBoardId, countBoard);

            return ResultT<int>.Success(countBoard);
        }

        logger.LogError("Invalid request: The request object is null.");

        return ResultT<int>.Failure(Error.Failure("400", "Invalid request."));
    }
}