using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressBoard.Query.Pagination
{
    internal sealed class GetPagedProgressBoardQuerysHandler(
        IProgressBoardRepository progressBoardRepository,
        ILogger<GetPagedProgressBoardQuerysHandler> logger)
        : IQueryHandler<GetPagedProgressBoardQuery, PagedResult<ProgressBoardDTos>>
    {
        public async Task<ResultT<PagedResult<ProgressBoardDTos>>> Handle(
            GetPagedProgressBoardQuery request, 
            CancellationToken cancellationToken)
        {

            if (request != null)
            {
                var pageProgressBoard = await progressBoardRepository.GetPagedBoardsAsync(request.PageNumber, request.PageSize,cancellationToken);
                var progressBoardList = pageProgressBoard.Items!.Select(x => new ProgressBoardDTos
                (
                    ProgressBoardId: x.Id,
                    CommunitiesId: x.CommunitiesId,
                    CreatedAt: x.CreatedAt,
                    UpdatedAt: x.UpdatedAt
                ));

                IEnumerable<ProgressBoardDTos> progressBoardDTosEnumerable = progressBoardList.ToList();
                if (progressBoardList == null ||!progressBoardDTosEnumerable.Any())
                {
                    logger.LogWarning("No progress boards found for page {PageNumber} with page size {PageSize}.", 
                        request.PageNumber, 
                        request.PageSize);

                    return ResultT<PagedResult<ProgressBoardDTos>>.Failure(Error.Failure("400", "The list is empty"));
                }

                PagedResult<ProgressBoardDTos> result = new()
                {
                    TotalItems = pageProgressBoard.TotalItems,
                    CurrentPage = pageProgressBoard.CurrentPage,
                    TotalPages = pageProgressBoard.TotalPages,
                    Items = progressBoardDTosEnumerable
                };

                logger.LogInformation("Retrieved {TotalItems} progress boards for page {PageNumber}.", 
                    pageProgressBoard.TotalItems, 
                    request.PageNumber);

                return ResultT<PagedResult<ProgressBoardDTos>>.Success(result);
            }
            logger.LogError("Failed to retrieve progress boards. Request object is null.");

            return ResultT<PagedResult<ProgressBoardDTos>>.Failure(Error.Failure("400", "Invalid request data"));
        }
    }
}
