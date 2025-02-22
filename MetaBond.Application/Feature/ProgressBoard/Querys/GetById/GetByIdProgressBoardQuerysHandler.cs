using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using MetaBond.Domain.Models;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressBoard.Querys.GetById
{
    internal sealed class GetByIdProgressBoardQuerysHandler : IQueryHandler<GetByIdProgressBoardQuerys, ProgressBoardDTos>
    {
        private readonly IProgressBoardRepository _progressBoardRepository;
        private readonly ILogger<GetByIdProgressBoardQuerysHandler> _logger;

        public GetByIdProgressBoardQuerysHandler(
            IProgressBoardRepository progressBoardRepository, 
            ILogger<GetByIdProgressBoardQuerysHandler> logger)
        {
            _progressBoardRepository = progressBoardRepository;
            _logger = logger;
        }

        public async Task<ResultT<ProgressBoardDTos>> Handle(
            GetByIdProgressBoardQuerys request, 
            CancellationToken cancellationToken)
        {
            var progressBoard = await _progressBoardRepository.GetByIdAsync(request.ProgressBoardId);
            if (progressBoard != null)
            {
                ProgressBoardDTos progressBoardDTos = new
                (
                    ProgressBoardId: progressBoard.Id,
                    CommunitiesId: progressBoard.CommunitiesId,
                    CreatedAt: progressBoard.CreatedAt,
                    UpdatedAt: progressBoard.UpdatedAt
                );

                _logger.LogInformation("Progress board retrieved successfully. ID: {ProgressBoardId}", progressBoard.Id);

                return ResultT<ProgressBoardDTos>.Success(progressBoardDTos);
            }
            _logger.LogError("Failed to retrieve progress board. ID: {ProgressBoardId} not found.", request.ProgressBoardId);

            return ResultT<ProgressBoardDTos>.Failure(Error.NotFound("404", $"Progress board with ID {request.ProgressBoardId} not found"));
        }
    }
}
