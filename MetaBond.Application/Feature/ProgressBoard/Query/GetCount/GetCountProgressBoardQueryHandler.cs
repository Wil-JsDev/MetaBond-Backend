using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressBoard.Query.GetCount;

internal sealed class GetCountProgressBoardQueryHandler(
    IProgressBoardRepository progressBoardRepository,
    ILogger<GetCountProgressBoardQueryHandler> logger)
    : IQueryHandler<GetCountProgressBoardQuery, int>
{
    public async Task<ResultT<int>> Handle(
        GetCountProgressBoardQuery request,
        CancellationToken cancellationToken)
    {
        var progressBoardCount = await progressBoardRepository.CountBoardsAsync(cancellationToken);

        if (progressBoardCount == 0)
        {
            logger.LogError("No progress boards found in the database.");

            return ResultT<int>.Failure(Error.Failure("400", "No progress boards available"));
        }

        logger.LogInformation("Total progress boards counted: {Count}", progressBoardCount);

        return ResultT<int>.Success(progressBoardCount);
    }
}