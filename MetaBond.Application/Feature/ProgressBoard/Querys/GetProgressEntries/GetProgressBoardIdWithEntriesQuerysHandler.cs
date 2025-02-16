using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressBoard.Querys.GetProgressEntries
{
    internal sealed class GetProgressBoardIdWithEntriesQuerysHandler : IQueryHandler<GetProgressBoardIdWithEntriesQuerys, IEnumerable<ProgressBoardWithProgressEntryDTos>>
    {
        private readonly IProgressBoardRepository _repository;
        private readonly ILogger<GetProgressBoardIdWithEntriesQuerysHandler> _logger;

        public GetProgressBoardIdWithEntriesQuerysHandler(
            IProgressBoardRepository repository, 
            ILogger<GetProgressBoardIdWithEntriesQuerysHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ResultT<IEnumerable<ProgressBoardWithProgressEntryDTos>>> Handle(
            GetProgressBoardIdWithEntriesQuerys request, 
            CancellationToken cancellationToken)
        {

            var progressBoard = await _repository.GetByIdAsync(request.ProgressBoardId);
            if (progressBoard != null)
            {
                var progressBoardList = await _repository.GetBoardsWithEntriesAsync(progressBoard.Id,cancellationToken);
                if (progressBoardList == null || !progressBoardList.Any())
                {
                    _logger.LogError("No progress entries found for ProgressBoardId: {ProgressBoardId}", request.ProgressBoardId);

                    return ResultT<IEnumerable<ProgressBoardWithProgressEntryDTos>>.Failure(Error.Failure("400", "The list is empty"));
                }

                IEnumerable<ProgressBoardWithProgressEntryDTos> progressBoardWithProgressEntryDs = progressBoardList.Select(x => new ProgressBoardWithProgressEntryDTos
                (
                    ProgressBoardId: x.Id,
                    CommunitiesId: x.CommunitiesId,
                    ProgressEntries: x.ProgressEntries,
                    CreatedAt: x.CreatedAt,
                    UpdatedAt: x.UpdatedAt
                ));

                _logger.LogInformation("Successfully retrieved {Count} progress entries for ProgressBoardId: {ProgressBoardId}",
                 progressBoardWithProgressEntryDs.Count(), request.ProgressBoardId);

                return ResultT<IEnumerable<ProgressBoardWithProgressEntryDTos>>.Success(progressBoardWithProgressEntryDs);
            }
            _logger.LogError("ProgressBoard with ID {ProgressBoardId} not found.", request.ProgressBoardId);

            return ResultT<IEnumerable<ProgressBoardWithProgressEntryDTos>>.Failure(Error.NotFound("404", "Progress board not found"));
        }
    }
}
