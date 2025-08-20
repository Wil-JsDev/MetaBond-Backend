using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressBoard.Query.Pagination;

internal sealed class GetPagedProgressBoardQueryHandler(
    IProgressBoardRepository progressBoardRepository,
    IDistributedCache decoratedCache,
    ILogger<GetPagedProgressBoardQueryHandler> logger)
    : IQueryHandler<GetPagedProgressBoardQuery, PagedResult<ProgressBoardDTos>>
{
    public async Task<ResultT<PagedResult<ProgressBoardDTos>>> Handle(
        GetPagedProgressBoardQuery request,
        CancellationToken cancellationToken)
    {
        string cacheKey = $"get-progress-board-paged-{request.PageNumber}-{request.PageSize}";
        var pageProgressBoard = await decoratedCache.GetOrCreateAsync(
            cacheKey,
            async () =>
            {
                var paged = await progressBoardRepository.GetPagedBoardsAsync(
                    request.PageNumber,
                    request.PageSize,
                    cancellationToken);

                var progressBoardList = paged.Items!.Select(ProgressBoardMapper.ProgressBoardToDto);

                PagedResult<ProgressBoardDTos> result = new()
                {
                    TotalItems = paged.TotalItems,
                    CurrentPage = paged.CurrentPage,
                    TotalPages = paged.TotalPages,
                    Items = progressBoardList
                };

                return result;
            },
            cancellationToken: cancellationToken);

        if (!pageProgressBoard.Items!.Any())
        {
            logger.LogWarning("No progress boards found for page {PageNumber} with page size {PageSize}.",
                request.PageNumber,
                request.PageSize);

            return ResultT<PagedResult<ProgressBoardDTos>>.Failure(Error.Failure("400", "The list is empty"));
        }

        logger.LogInformation("Retrieved {TotalItems} progress boards for page {PageNumber}.",
            pageProgressBoard.TotalItems,
            request.PageNumber);

        return ResultT<PagedResult<ProgressBoardDTos>>.Success(pageProgressBoard);
    }
}