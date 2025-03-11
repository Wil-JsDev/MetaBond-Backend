using System.Security.Cryptography;
using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressEntry.Querys.GetByIdProgressEntryWithProgressBoard;

public class GetProgressEntryWithBoardByIdQueryHandler : IQueryHandler<GetProgressEntryWithBoardByIdQuery, IEnumerable<ProgressEntryWithProgressBoardDTos>>
{
    private readonly IProgressEntryRepository _repository;
    private readonly ILogger<GetProgressEntryWithBoardByIdQueryHandler> _logger;
    
    public GetProgressEntryWithBoardByIdQueryHandler(
        IProgressEntryRepository repository, 
        ILogger<GetProgressEntryWithBoardByIdQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public async Task<ResultT<IEnumerable<ProgressEntryWithProgressBoardDTos>>> Handle(
        GetProgressEntryWithBoardByIdQuery request, 
        CancellationToken cancellationToken)
    {
        var progressEntry = await _repository.GetByIdAsync(request.ProgressEntryId);
        if (progressEntry != null)
        {
          var progressEntryWithProgressBoard = await _repository.GetByIdProgressEntryWithProgressBoard(request.ProgressEntryId,cancellationToken);
          if (!progressEntryWithProgressBoard.Any())
          {
              _logger.LogError("No progress board entries found for ProgressEntry ID {ProgressEntryId}", request.ProgressEntryId);
              return ResultT<IEnumerable<ProgressEntryWithProgressBoardDTos>>.Failure(Error.Failure("400", "No related progress board entries found."));
          }

          IEnumerable<ProgressEntryWithProgressBoardDTos> progressEntryWithBoardDTos = progressEntryWithProgressBoard.Select(x => new ProgressEntryWithProgressBoardDTos
          (
              ProgressEntryId: x.Id,
              ProgressBoard: x.ProgressBoard,
              Description: x.Description,
              CreatedAt: x.CreatedAt,
              UpdateAt: x.UpdateAt
          ));
          
          _logger.LogInformation("Successfully retrieved {Count} progress board entries for ProgressEntry ID {ProgressEntryId}", 
              progressEntryWithBoardDTos.Count(), request.ProgressEntryId);

          return ResultT<IEnumerable<ProgressEntryWithProgressBoardDTos>>.Success(progressEntryWithBoardDTos);
        }
        _logger.LogError("Progress entry with ID {ProgressEntryId} not found.", request.ProgressEntryId);
        
        return ResultT<IEnumerable<ProgressEntryWithProgressBoardDTos>>.Failure(Error.NotFound("404", "Progress entry not found."));
    }
}