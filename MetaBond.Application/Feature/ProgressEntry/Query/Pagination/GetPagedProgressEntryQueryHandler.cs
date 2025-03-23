using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressEntry.Query.Pagination;

internal sealed class GetPagedProgressEntryQueryHandler(
    IProgressEntryRepository progressEntryRepository,
    ILogger<GetPagedProgressEntryQueryHandler> logger)
    : IQueryHandler<GetPagedProgressEntryQuery, PagedResult<ProgressEntryDTos>>
{
    public async Task<ResultT<PagedResult<ProgressEntryDTos>>> Handle(
        GetPagedProgressEntryQuery request, 
        CancellationToken cancellationToken)
    {
        if (request != null)
        {

            if (request.PageNumber <= 0 || request.PageSize <= 0)
            {
                logger.LogError("");

                return ResultT<PagedResult<ProgressEntryDTos>>.Failure(Error.Failure("400", "Page Number and Page Size are required"));
            }
                
            var getPagedProgressEntry = await progressEntryRepository.GetPagedProgressEntryAsync(request.PageSize,request.PageNumber,cancellationToken);

            var dtoItems = getPagedProgressEntry.Items!.Select(x => new ProgressEntryDTos
            (
                ProgressEntryId: x.Id,
                ProgressBoardId: x.ProgressBoardId,
                Description: x.Description,
                CreatedAt: x.CreatedAt,
                UpdateAt: x.UpdateAt
            ));
            IEnumerable<ProgressEntryDTos> progressEntryDTosEnumerable = dtoItems.ToList();
            if (!progressEntryDTosEnumerable.Any())
            {
                logger.LogError("No progress entries found for the given page request. Page: {PageNumber}, Size: {PageSize}", 
                    request.PageNumber, request.PageSize);

                return ResultT<PagedResult<ProgressEntryDTos>>.Failure(Error.Failure("400", "The list is empty"));
            }

            PagedResult<ProgressEntryDTos> result = new()
            {
                TotalItems = getPagedProgressEntry.TotalItems,
                CurrentPage = getPagedProgressEntry.CurrentPage,
                TotalPages = getPagedProgressEntry.TotalPages,
                Items = progressEntryDTosEnumerable
            };

            logger.LogInformation("Successfully retrieved {TotalItems} progress entries for page {PageNumber} with page size {PageSize}.", 
                getPagedProgressEntry.TotalItems, request.PageNumber, request.PageSize);

            return ResultT<PagedResult<ProgressEntryDTos>>.Success(result);
        }

        logger.LogError("Invalid request received. Page: {PageNumber}, Size: {PageSize}", request!.PageNumber, request.PageSize);

        return ResultT<PagedResult<ProgressEntryDTos>>.Failure(Error.Failure("400", "Invalid request"));
    }
}