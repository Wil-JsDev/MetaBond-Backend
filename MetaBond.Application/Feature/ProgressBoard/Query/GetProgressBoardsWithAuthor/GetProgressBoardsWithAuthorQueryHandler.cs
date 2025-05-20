using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.DTOs.Communities;
using MetaBond.Application.DTOs.ProgressBoard;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressBoard.Query.GetProgressBoardsWithAuthor;

internal sealed class GetProgressBoardsWithAuthorQueryHandler(
    IProgressBoardRepository progressBoardRepository,
    ILogger<GetProgressBoardsWithAuthorQueryHandler> logger,
    IDistributedCache decorated): 
    IQueryHandler<GetProgressBoardsWithAuthorQuery, 
        IEnumerable<ProgressBoardWithUserDTos>>
{
    public async Task<ResultT<IEnumerable<ProgressBoardWithUserDTos>>> Handle
    (GetProgressBoardsWithAuthorQuery request, 
            CancellationToken cancellationToken)
    {

        if (request != null)
        {
            var progressBoardId = await progressBoardRepository.GetByIdAsync(request.ProgressBoardId);
            if (progressBoardId == null)
            {
                logger.LogWarning($"ProgressBoard with id: {request.ProgressBoardId} not found");

                return ResultT<IEnumerable<ProgressBoardWithUserDTos>>.Failure(Error.NotFound("404", "ProgressBoard not found"));
            }

            var progressBoards = await decorated.GetOrCreateAsync($"GetProgressBoardsWithAuthor_{request.ProgressBoardId}",
                async () => await progressBoardRepository.GetProgressBoardsWithAuthorAsync(
                    progressBoardId.Id,
                    cancellationToken),
                cancellationToken: cancellationToken);

            IEnumerable<Domain.Models.ProgressBoard> enumerable = progressBoards.ToList();

            if (!enumerable.Any())
            {
                logger.LogWarning($"No ProgressBoards found with id: {request.ProgressBoardId}");

                return ResultT<IEnumerable<ProgressBoardWithUserDTos>>.Failure(Error.NotFound("404", "No ProgressBoards found"));
            }

            var boardsWithAuthor = enumerable.Select(ProgressBoardMapper.ToDTo);
            
            var progressBoardWithUserDTosEnumerable = boardsWithAuthor.ToList();
            
            logger.LogInformation($"Successfully retrieved {progressBoardWithUserDTosEnumerable.Count()} progress boards with authors for ProgressBoardId: {request.ProgressBoardId}");

            return ResultT<IEnumerable<ProgressBoardWithUserDTos>>.Success(progressBoardWithUserDTosEnumerable);
        }
        
        logger.LogWarning("Received null request when attempting to retrieve progress boards with authors");

        return ResultT<IEnumerable<ProgressBoardWithUserDTos>>.Failure(Error.Failure("400", "Request cannot be null"));
    }
}