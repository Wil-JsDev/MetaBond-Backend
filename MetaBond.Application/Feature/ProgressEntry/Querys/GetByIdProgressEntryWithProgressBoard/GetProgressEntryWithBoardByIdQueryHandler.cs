using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressEntry.Querys.GetByIdProgressEntryWithProgressBoard;

public class GetProgressEntryWithBoardByIdQueryHandler(
    IProgressEntryRepository repository,
    ILogger<GetProgressEntryWithBoardByIdQueryHandler> logger)
    : IQueryHandler<GetProgressEntryWithBoardByIdQuery, IEnumerable<ProgressEntryWithProgressBoardDTos>>
{
    public async Task<ResultT<IEnumerable<ProgressEntryWithProgressBoardDTos>>> Handle(
        GetProgressEntryWithBoardByIdQuery request, 
        CancellationToken cancellationToken)
    {
        var progressEntry = await repository.GetByIdAsync(request.ProgressEntryId);
        if (progressEntry != null)
        {
          var progressEntryWithProgressBoard = await repository.GetByIdProgressEntryWithProgressBoard(request.ProgressEntryId,cancellationToken);
          
          IEnumerable<Domain.Models.ProgressEntry> entryWithProgressBoard = progressEntryWithProgressBoard.ToList();
          if (!entryWithProgressBoard.Any())
          {
              logger.LogError("No progress board entries found for ProgressEntry ID {ProgressEntryId}", request.ProgressEntryId);
              return ResultT<IEnumerable<ProgressEntryWithProgressBoardDTos>>.Failure(Error.Failure("400", "No related progress board entries found."));
          }

          IEnumerable<ProgressEntryWithProgressBoardDTos> progressEntryWithBoardDTos = entryWithProgressBoard.Select(x => new ProgressEntryWithProgressBoardDTos
          (
              ProgressEntryId: x.Id,
              ProgressBoard: new ProgressBoardSummaryDTos
              (
                  ProgressBoardId: x.ProgressBoard!.Id,
                  CommunitiesId: x.ProgressBoard.CommunitiesId, 
                  CreatedAt: x.ProgressBoard.CreatedAt,
                  ModifiedAt: x.ProgressBoard.UpdatedAt
              ),
              Description: x.Description,
              CreatedAt: x.CreatedAt,
              UpdateAt: x.UpdateAt
          ));

          IEnumerable<ProgressEntryWithProgressBoardDTos> progressEntryWithProgressBoardDTosEnumerable = progressEntryWithBoardDTos.ToList();
          
          logger.LogInformation("Successfully retrieved {Count} progress board entries for ProgressEntry ID {ProgressEntryId}", 
              progressEntryWithProgressBoardDTosEnumerable.Count(), request.ProgressEntryId);

          return ResultT<IEnumerable<ProgressEntryWithProgressBoardDTos>>.Success(progressEntryWithProgressBoardDTosEnumerable);
        }
        logger.LogError("Progress entry with ID {ProgressEntryId} not found.", request.ProgressEntryId);
        
        return ResultT<IEnumerable<ProgressEntryWithProgressBoardDTos>>.Failure(Error.NotFound("404", "Progress entry not found."));
    }
}