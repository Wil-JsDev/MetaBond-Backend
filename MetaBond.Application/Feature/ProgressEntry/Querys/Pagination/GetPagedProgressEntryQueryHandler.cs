using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressEntry.Querys.Pagination
{
    internal sealed class GetPagedProgressEntryQueryHandler : IQueryHandler<GetPagedProgressEntryQuery, PagedResult<ProgressEntryDTos>>
    {
        private readonly IProgressEntryRepository _progressEntryRepository;
        private readonly ILogger<GetPagedProgressEntryQueryHandler> _logger;

        public GetPagedProgressEntryQueryHandler(
            IProgressEntryRepository progressEntryRepository, 
            ILogger<GetPagedProgressEntryQueryHandler> logger)
        {
            _progressEntryRepository = progressEntryRepository;
            _logger = logger;
        }

        public async Task<ResultT<PagedResult<ProgressEntryDTos>>> Handle(
            GetPagedProgressEntryQuery request, 
            CancellationToken cancellationToken)
        {
            if (request != null)
            {
                var getPagedProgressEntry = await _progressEntryRepository.GetPagedProgressEntryAsync(request.PageSize,request.PageNumber,cancellationToken);

                var dtoItems = getPagedProgressEntry.Items.Select(x => new ProgressEntryDTos
                (
                     ProgressEntryId: x.Id,
                    ProgressBoardId: x.ProgressBoardId,
                    Description: x.Description,
                    CreatedAt: x.CreatedAt,
                    UpdateAt: x.UpdateAt
                ));
                if (!dtoItems.Any())
                {
                    _logger.LogError("No progress entries found for the given page request. Page: {PageNumber}, Size: {PageSize}", 
                        request.PageNumber, request.PageSize);

                    return ResultT<PagedResult<ProgressEntryDTos>>.Failure(Error.Failure("400", "The list is empty"));
                }

                PagedResult<ProgressEntryDTos> result = new()
                {
                    TotalItems = getPagedProgressEntry.TotalItems,
                    CurrentPage = getPagedProgressEntry.CurrentPage,
                    TotalPages = getPagedProgressEntry.TotalPages,
                    Items = dtoItems
                };

                _logger.LogInformation("Successfully retrieved {TotalItems} progress entries for page {PageNumber} with page size {PageSize}.", 
                    getPagedProgressEntry.TotalItems, request.PageNumber, request.PageSize);

                return ResultT<PagedResult<ProgressEntryDTos>>.Success(result);
            }

            _logger.LogError("Invalid request received. Page: {PageNumber}, Size: {PageSize}", request.PageNumber, request.PageSize);

            return ResultT<PagedResult<ProgressEntryDTos>>.Failure(Error.Failure("400", "Invalid request"));
        }
    }
}
