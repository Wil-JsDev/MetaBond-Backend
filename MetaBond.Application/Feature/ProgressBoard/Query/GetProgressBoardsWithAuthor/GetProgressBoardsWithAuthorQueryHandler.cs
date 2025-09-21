using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.DTOs.Communities;
using MetaBond.Application.DTOs.ProgressBoard;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressBoard.Query.GetProgressBoardsWithAuthor;

internal sealed class GetProgressBoardsWithAuthorQueryHandler(
    IProgressBoardRepository progressBoardRepository,
    ILogger<GetProgressBoardsWithAuthorQueryHandler> logger,
    IDistributedCache decorated) :
    IQueryHandler<GetProgressBoardsWithAuthorQuery,
        PagedResult<ProgressBoardWithUserDTos>>
{
    public async Task<ResultT<PagedResult<ProgressBoardWithUserDTos>>> Handle
    (GetProgressBoardsWithAuthorQuery request,
        CancellationToken cancellationToken)
    {
        var progressBoard = await EntityHelper.GetEntityByIdAsync(
            progressBoardRepository.GetByIdAsync,
            request.ProgressBoardId,
            "ProgressBoard",
            logger
        );

        if (!progressBoard.IsSuccess) return progressBoard.Error!;

        var paginationValidation = PaginationHelper.ValidatePagination<ProgressBoardWithUserDTos>(
            request.PageNumber, request.PageSize, logger);

        if (!paginationValidation.IsSuccess) return paginationValidation.Error!;

        var progressBoards = await decorated.GetOrCreateAsync(
            $"GetProgressBoardsWithAuthor_{request.ProgressBoardId}-page-{request.PageNumber}-s-{request.PageSize}",
            async () =>
            {
                var progressBoardList = await progressBoardRepository.GetProgressBoardsWithAuthorAsync(
                    progressBoard.Value.Id,
                    request.PageNumber,
                    request.PageSize,
                    cancellationToken);

                var items = progressBoardList.Items ?? [];

                var boardsWithAuthor = items.Select(ProgressBoardMapper.ToDTo);

                PagedResult<ProgressBoardWithUserDTos> pagedResult = new()
                {
                    TotalItems = progressBoardList.TotalItems,
                    CurrentPage = progressBoardList.CurrentPage,
                    TotalPages = progressBoardList.TotalPages,
                    Items = boardsWithAuthor
                };

                return pagedResult;
            },
            cancellationToken: cancellationToken);


        var itemsDto = progressBoards.Items ?? [];
        var boardWithUserDTosEnumerable = itemsDto.ToList();

        if (!boardWithUserDTosEnumerable.Any())
        {
            logger.LogWarning($"No ProgressBoards found with id: {request.ProgressBoardId}");

            return ResultT<PagedResult<ProgressBoardWithUserDTos>>.Failure(Error.NotFound("404",
                "No ProgressBoards found"));
        }

        logger.LogInformation(
            "Successfully retrieved {Count} progress boards with authors for ProgressBoardId: {RequestProgressBoardId}",
            boardWithUserDTosEnumerable.Count(), request.ProgressBoardId);

        return ResultT<PagedResult<ProgressBoardWithUserDTos>>.Success(progressBoards);
    }
}