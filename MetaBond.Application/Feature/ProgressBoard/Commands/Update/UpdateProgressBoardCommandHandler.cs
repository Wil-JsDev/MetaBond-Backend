using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressBoard.Commands.Update
{
    internal sealed class UpdateProgressBoardCommandHandler : ICommandHandler<UpdateProgressBoardCommand, ProgressBoardDTos>
    {
        private readonly IProgressBoardRepository _progressBoardRepository;
        private readonly ILogger<UpdateProgressBoardCommandHandler> _logger;

        public UpdateProgressBoardCommandHandler(
            IProgressBoardRepository progressBoardRepository, 
            ILogger<UpdateProgressBoardCommandHandler> logger)
        {
            _progressBoardRepository = progressBoardRepository;
            _logger = logger;
        }

        public async Task<ResultT<ProgressBoardDTos>> Handle(
            UpdateProgressBoardCommand request, 
            CancellationToken cancellationToken)
        {

            var progressBoard = await _progressBoardRepository.GetByIdAsync(request.ProgressBoardId);
            if (progressBoard != null)
            {
                progressBoard.CommunitiesId = request.CommunitiesId;
                progressBoard.UpdatedAt = DateTime.UtcNow;

                await _progressBoardRepository.UpdateAsync(progressBoard,cancellationToken);

                _logger.LogInformation("Progress board with ID: {ProgressBoardId} updated successfully.", progressBoard.Id);

                ProgressBoardDTos progressBoardDTos = new
                (
                    ProgressBoardId: progressBoard.Id,
                    CommunitiesId: progressBoard.CommunitiesId,
                    CreatedAt: progressBoard.CreatedAt,
                    UpdatedAt: progressBoard.UpdatedAt
                );

                return ResultT<ProgressBoardDTos>.Success(progressBoardDTos);

            }
            _logger.LogError("Failed to update progress board. ID: {ProgressBoardId} not found.", request.ProgressBoardId);

            return ResultT<ProgressBoardDTos>.Failure(Error.NotFound("404", $"Progress board with ID {request.ProgressBoardId} not found"));
        }
    }
}
