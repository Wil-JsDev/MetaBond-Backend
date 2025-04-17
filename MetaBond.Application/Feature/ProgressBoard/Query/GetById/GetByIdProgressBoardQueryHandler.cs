using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressBoard.Query.GetById;

internal sealed class GetByIdProgressBoardQueryHandler(
    IProgressBoardRepository progressBoardRepository,
    IDistributedCache decoratedCache,
    ILogger<GetByIdProgressBoardQueryHandler> logger)
    : IQueryHandler<GetByIdProgressBoardQuery, ProgressBoardDTos>
{
    public async Task<ResultT<ProgressBoardDTos>> Handle(
        GetByIdProgressBoardQuery request, 
        CancellationToken cancellationToken)
    {
        var progressBoard = await decoratedCache.GetOrCreateAsync(
            $"progress-board-{request.ProgressBoardId}",
            async () => await progressBoardRepository.GetByIdAsync(request.ProgressBoardId), 
            cancellationToken: cancellationToken);
        
        if (progressBoard != null)
        {
            ProgressBoardDTos progressBoardDTos = new
            (
                ProgressBoardId: progressBoard.Id,
                CommunitiesId: progressBoard.CommunitiesId,
                CreatedAt: progressBoard.CreatedAt,
                UpdatedAt: progressBoard.UpdatedAt
            );

            logger.LogInformation("Progress board retrieved successfully. ID: {ProgressBoardId}", progressBoard.Id);

            return ResultT<ProgressBoardDTos>.Success(progressBoardDTos);
        }
        logger.LogError("Failed to retrieve progress board. ID: {ProgressBoardId} not found.", request.ProgressBoardId);

        return ResultT<ProgressBoardDTos>.Failure(Error.NotFound("404", $"Progress board with ID {request.ProgressBoardId} not found"));
    }
}