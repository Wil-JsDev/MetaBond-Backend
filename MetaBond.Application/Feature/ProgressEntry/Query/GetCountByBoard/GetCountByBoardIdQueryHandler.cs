using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressEntry.Querys.GetCountByBoard
{
    internal sealed class GetCountByBoardIdQueryHandler : IQueryHandler<GetCountByBoardIdQuery, int>
    {
        private readonly IProgressEntryRepository _progressEntryRepository;
        private readonly ILogger<GetCountByBoardIdQueryHandler> _logger;

        public GetCountByBoardIdQueryHandler(
            IProgressEntryRepository progressEntryRepository, 
            ILogger<GetCountByBoardIdQueryHandler> logger)
        {
            _progressEntryRepository = progressEntryRepository;
            _logger = logger;
        }

        public async Task<ResultT<int>> Handle(
            GetCountByBoardIdQuery request, 
            CancellationToken cancellationToken)
        {

            if (request != null)
            {
                var countBoard = await _progressEntryRepository.CountEntriesByBoardIdAsync(request.ProgressBoardId,cancellationToken);

                _logger.LogInformation("Successfully retrieved the count of progress entries for ProgressBoardId: {BoardId}. Count: {Count}", 
                    request.ProgressBoardId, countBoard);

                return ResultT<int>.Success(countBoard);
            }

            _logger.LogError("Invalid request: The request object is null.");

            return ResultT<int>.Failure(Error.Failure("400", "Invalid request."));
        }
    }
}
