using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressBoard.Query.GetById;

internal sealed class GetByIdProgressBoardQueryHandler(
    IProgressBoardRepository progressBoardRepository,
    ILogger<GetByIdProgressBoardQueryHandler> logger)
    : IQueryHandler<GetByIdProgressBoardQuery, ProgressBoardDTos>
{
    public async Task<ResultT<ProgressBoardDTos>> Handle(
        GetByIdProgressBoardQuery request,
        CancellationToken cancellationToken)
    {
        var progressBoard = await EntityHelper.GetEntityByIdAsync(
            progressBoardRepository.GetByIdAsync,
            request.ProgressBoardId,
            "ProgressBoard",
            logger
        );

        if (!progressBoard.IsSuccess)
        {
            logger.LogError("Failed to retrieve progress board. ID: {ProgressBoardId} not found.",
                request.ProgressBoardId);

            return ResultT<ProgressBoardDTos>.Failure(Error.NotFound("404",
                $"Progress board with ID {request.ProgressBoardId} not found"));
        }

        var progressBoardDTos = ProgressBoardMapper.ProgressBoardToDto(progressBoard.Value);

        logger.LogInformation("Progress board retrieved successfully. ID: {ProgressBoardId}", progressBoard.Value.Id);

        return ResultT<ProgressBoardDTos>.Success(progressBoardDTos);
    }
}