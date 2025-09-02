using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressBoard.Commands.Update;

internal sealed class UpdateProgressBoardCommandHandler(
    IProgressBoardRepository progressBoardRepository,
    ILogger<UpdateProgressBoardCommandHandler> logger)
    : ICommandHandler<UpdateProgressBoardCommand, ProgressBoardDTos>
{
    public async Task<ResultT<ProgressBoardDTos>> Handle(
        UpdateProgressBoardCommand request,
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
            logger.LogError("Failed to update progress board. ID: {ProgressBoardId} not found.",
                request.ProgressBoardId);

            return ResultT<ProgressBoardDTos>.Failure(Error.NotFound("404",
                $"Progress board with ID {request.ProgressBoardId} not found"));
        }

        progressBoard.Value.CommunitiesId = request.CommunitiesId;
        progressBoard.Value.UpdatedAt = DateTime.UtcNow;

        await progressBoardRepository.UpdateAsync(progressBoard.Value, cancellationToken);

        logger.LogInformation("Progress board with ID: {ProgressBoardId} updated successfully.",
            progressBoard.Value.Id);

        var progressBoardDTos = ProgressBoardMapper.ProgressBoardToDto(progressBoard.Value);

        return ResultT<ProgressBoardDTos>.Success(progressBoardDTos);
    }
}