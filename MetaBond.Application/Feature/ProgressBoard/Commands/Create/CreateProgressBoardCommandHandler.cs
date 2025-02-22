using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using MetaBond.Domain.Models;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressBoard.Commands.Create
{
    internal sealed class CreateProgressBoardCommandHandler : ICommandHandler<CreateProgressBoardCommand, ProgressBoardDTos>
    {
        private readonly IProgressBoardRepository _progressBoardRepository;
        private readonly ILogger<CreateProgressBoardCommandHandler> _logger;

        public CreateProgressBoardCommandHandler(
            IProgressBoardRepository progressBoardRepository,
            ILogger<CreateProgressBoardCommandHandler> logger)
        {
            _progressBoardRepository = progressBoardRepository;
            _logger = logger;
        }

        public async Task<ResultT<ProgressBoardDTos>> Handle(
            CreateProgressBoardCommand request,
            CancellationToken cancellationToken)
        {

            if (request != null)
            {
                Domain.Models.ProgressBoard progressBoard = new()
                {
                    Id = Guid.NewGuid(),
                    CommunitiesId = request.CommunitiesId,
                    UpdatedAt = DateTime.UtcNow,
                };

                await _progressBoardRepository.CreateAsync(progressBoard, cancellationToken);

                _logger.LogInformation("Progress board created successfully with ID: {ProgressBoardId}", progressBoard.Id);

                ProgressBoardDTos progressBoardDTos = new
                (
                    ProgressBoardId: progressBoard.Id,
                    CommunitiesId: progressBoard.CommunitiesId,
                    CreatedAt: progressBoard.CreatedAt,
                    UpdatedAt: progressBoard.UpdatedAt
                );

                return ResultT<ProgressBoardDTos>.Success(progressBoardDTos);

            }
            _logger.LogError("Failed to create progress board. Request is null.");

            return ResultT<ProgressBoardDTos>.Failure(Error.Failure("400", "Invalid request data"));
        }
    }
}
