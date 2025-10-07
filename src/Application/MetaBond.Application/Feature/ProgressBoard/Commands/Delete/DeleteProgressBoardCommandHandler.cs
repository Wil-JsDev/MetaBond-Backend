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
            if (!progressBoard.IsSuccess) return progressBoard.Error!;

            await repository.DeleteAsync(progressBoard.Value, cancellationToken);

            logger.LogInformation("Progress board with ID: {ProgressBoardId} deleted successfully.",
                progressBoard.Value.Id);

            return ResultT<Guid>.Success(progressBoard.Value.Id);
        }
    }
}