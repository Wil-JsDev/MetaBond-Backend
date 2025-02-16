
using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressBoard.Commands.Delete
{
    internal sealed class DeleteProgressBoardCommandHandler : ICommandHandler<DeleteProgressBoardCommand, Guid>
    {
        private readonly IProgressBoardRepository _repository;
        private readonly ILogger<DeleteProgressBoardCommandHandler> _logger;

        public DeleteProgressBoardCommandHandler(
            IProgressBoardRepository repository, 
            ILogger<DeleteProgressBoardCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ResultT<Guid>> Handle(
            DeleteProgressBoardCommand request, 
            CancellationToken cancellationToken)
        {

            var progressBoard = await _repository.GetByIdAsync(request.ProgressBoardId);
            if (progressBoard != null)
            {
                await _repository.DeleteAsync(progressBoard,cancellationToken);

                _logger.LogInformation("Progress board with ID: {ProgressBoardId} deleted successfully.", progressBoard.Id);

                return ResultT<Guid>.Success(progressBoard.Id);

            }
            _logger.LogError("Failed to delete progress board. ID: {ProgressBoardId} not found.", request.ProgressBoardId);

            return ResultT<Guid>.Failure(Error.NotFound("404", $"Progress board with ID {request.ProgressBoardId} not found"));
        }
    }
}
