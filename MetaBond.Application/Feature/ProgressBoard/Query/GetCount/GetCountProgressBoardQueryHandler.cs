using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressBoard.Query.GetCount;

internal sealed class GetCountProgressBoardQuerysHandler : IQueryHandler<GetCountProgressBoardQuery, int>
{
    private readonly IProgressBoardRepository _progressBoardRepository;
    private readonly ILogger<GetCountProgressBoardQuerysHandler> _logger;

    public GetCountProgressBoardQuerysHandler(
        IProgressBoardRepository progressBoardRepository, 
        ILogger<GetCountProgressBoardQuerysHandler> logger)
    {
        _progressBoardRepository = progressBoardRepository;
        _logger = logger;
    }

    public async Task<ResultT<int>> Handle(
        GetCountProgressBoardQuery request, 
        CancellationToken cancellationToken)
    {
        var progressBoardCount = await _progressBoardRepository.CountBoardsAsync(cancellationToken);
        if (progressBoardCount == 0)
        {
            _logger.LogError("No progress boards found in the database.");

            return ResultT<int>.Failure(Error.Failure("400", "No progress boards available"));
        }
        _logger.LogInformation("Total progress boards counted: {Count}", progressBoardCount);

        return ResultT<int>.Success(progressBoardCount);
    }
}