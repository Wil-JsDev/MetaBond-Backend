using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressBoard.Commands.Delete
{
    internal sealed class DeleteProgressBoardCommandHandler(
        IProgressBoardRepository repository,
        ILogger<DeleteProgressBoardCommandHandler> logger)
        : ICommandHandler<DeleteProgressBoardCommand, Guid>
    {
        public async Task<ResultT<Guid>> Handle(
            DeleteProgressBoardCommand request,
            CancellationToken cancellationToken)
        {
            var progressBoard = await EntityHelper.GetEntityByIdAsync(
                repository.GetByIdAsync,
                request.ProgressBoardId,
                "ProgressBoard",
                logger
            );
            if (progressBoard.IsSuccess)
            {
                await repository.DeleteAsync(progressBoard.Value, cancellationToken);

                logger.LogInformation("Progress board with ID: {ProgressBoardId} deleted successfully.",
                    progressBoard.Value.Id);

                return ResultT<Guid>.Success(progressBoard.Value.Id);
            }

            logger.LogError("Failed to delete progress board. ID: {ProgressBoardId} not found.",
                request.ProgressBoardId);

            return ResultT<Guid>.Failure(Error.NotFound("404",
                $"Progress board with ID {request.ProgressBoardId} not found"));
        }
    }
}