using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressEntry.Query.GetCountByBoard;

internal sealed class GetCountByBoardIdQueryHandler(
    IProgressEntryRepository progressEntryRepository,
    IProgressBoardRepository progressBoardRepository,
    ILogger<GetCountByBoardIdQueryHandler> logger)
    : IQueryHandler<GetCountByBoardIdQuery, int>
{
    public async Task<ResultT<int>> Handle(
        GetCountByBoardIdQuery request,
        CancellationToken cancellationToken)
    {
        var progressBoard = await progressBoardRepository.GetByIdAsync(request.ProgressBoardId);
        if (progressBoard is null)
        {
            logger.LogError($"No progress board found with id: {request.ProgressBoardId}");

            return ResultT<int>.Failure(Error.NotFound("404",
                "No progress board found"));
        }

        var countBoard = await progressEntryRepository.CountEntriesByBoardIdAsync(request.ProgressBoardId,
            cancellationToken);

        logger.LogInformation(
            "Successfully retrieved the count of progress entries for ProgressBoardId: {BoardId}. Count: {Count}",
            request.ProgressBoardId, countBoard);

        return ResultT<int>.Success(countBoard);
    }
}