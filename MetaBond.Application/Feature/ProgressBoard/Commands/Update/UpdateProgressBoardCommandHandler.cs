using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;
using MetaBond.Application.Interfaces.Repository;
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

        var progressBoard = await progressBoardRepository.GetByIdAsync(request.ProgressBoardId);
        if (progressBoard != null)
        {
            progressBoard.CommunitiesId = request.CommunitiesId;
            progressBoard.UpdatedAt = DateTime.UtcNow;

            await progressBoardRepository.UpdateAsync(progressBoard,cancellationToken);

            logger.LogInformation("Progress board with ID: {ProgressBoardId} updated successfully.", progressBoard.Id);

            ProgressBoardDTos progressBoardDTos = new
            (
                ProgressBoardId: progressBoard.Id,
                CommunitiesId: progressBoard.CommunitiesId,
                UserId:  progressBoard.UserId,
                CreatedAt: progressBoard.CreatedAt,
                UpdatedAt: progressBoard.UpdatedAt
            );

            return ResultT<ProgressBoardDTos>.Success(progressBoardDTos);

        }
        logger.LogError("Failed to update progress board. ID: {ProgressBoardId} not found.", request.ProgressBoardId);

        return ResultT<ProgressBoardDTos>.Failure(Error.NotFound("404", $"Progress board with ID {request.ProgressBoardId} not found"));
    }
}