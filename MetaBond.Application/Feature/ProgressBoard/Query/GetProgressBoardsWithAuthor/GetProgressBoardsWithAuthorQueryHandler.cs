using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.DTOs.Communities;
using MetaBond.Application.DTOs.ProgressBoard;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressBoard.Query.GetProgressBoardsWithAuthor;

internal sealed class GetProgressBoardsWithAuthorQueryHandler(
    IProgressBoardRepository progressBoardRepository,
    ILogger<GetProgressBoardsWithAuthorQueryHandler> logger,
    IDistributedCache decorated) :
    IQueryHandler<GetProgressBoardsWithAuthorQuery,
        IEnumerable<ProgressBoardWithUserDTos>>
{
    public async Task<ResultT<IEnumerable<ProgressBoardWithUserDTos>>> Handle
    (GetProgressBoardsWithAuthorQuery request,
        CancellationToken cancellationToken)
    {
        var progressBoard = await EntityHelper.GetEntityByIdAsync(
            progressBoardRepository.GetByIdAsync,
            request.ProgressBoardId,
            "ProgressBoard",
            logger
        );

        if (!progressBoard.IsSuccess)
        {
            logger.LogError("Failed to retrieve progress board. ID: {ProgressBoardId} not found.",
                request.ProgressBoardId);

            return ResultT<IEnumerable<ProgressBoardWithUserDTos>>.Failure(progressBoard.Error!);
        }

        var progressBoards = await decorated.GetOrCreateAsync($"GetProgressBoardsWithAuthor_{request.ProgressBoardId}",
            async () =>
            {
                var progressBoardList = await progressBoardRepository.GetProgressBoardsWithAuthorAsync(
                    progressBoard.Value.Id,
                    cancellationToken);

                var boardsWithAuthor = progressBoardList.Select(ProgressBoardMapper.ToDTo);

                return boardsWithAuthor;
            },
            cancellationToken: cancellationToken);

        IEnumerable<ProgressBoardWithUserDTos> boardWithUserDTosEnumerable = progressBoards.ToList();
        if (!boardWithUserDTosEnumerable.Any())
        {
            logger.LogWarning($"No ProgressBoards found with id: {request.ProgressBoardId}");

            return ResultT<IEnumerable<ProgressBoardWithUserDTos>>.Failure(Error.NotFound("404",
                "No ProgressBoards found"));
        }

        var progressBoardWithUserDTosEnumerable = boardWithUserDTosEnumerable.ToList();

        logger.LogInformation(
            $"Successfully retrieved {progressBoardWithUserDTosEnumerable.Count()} progress boards with authors for ProgressBoardId: {request.ProgressBoardId}");

        return ResultT<IEnumerable<ProgressBoardWithUserDTos>>.Success(progressBoardWithUserDTosEnumerable);
    }
}