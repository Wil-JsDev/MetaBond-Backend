using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressEntry.Query.Pagination;

internal sealed class GetPagedProgressEntryQueryHandler(
    IProgressEntryRepository progressEntryRepository,
    IDistributedCache decoratedCache,
    ILogger<GetPagedProgressEntryQueryHandler> logger)
    : IQueryHandler<GetPagedProgressEntryQuery, PagedResult<ProgressEntryDTos>>
{
    public async Task<ResultT<PagedResult<ProgressEntryDTos>>> Handle(
        GetPagedProgressEntryQuery request,
        CancellationToken cancellationToken)
    {
        if (request.PageNumber <= 0 || request.PageSize <= 0)
        {
            logger.LogError("Invalid pagination parameters: PageNumber={PageNumber}, PageSize={PageSize}",
                request.PageNumber, request.PageSize);

            return ResultT<PagedResult<ProgressEntryDTos>>.Failure(Error.Failure("400",
                "Page Number and Page Size must be greater than zero."));
        }

        var getPagedProgressEntry = await decoratedCache.GetOrCreateAsync(
            $"progress-entry-get-paged-{request.PageNumber}-size-{request.PageSize}",
            async () =>
            {
                var paged = await progressEntryRepository.GetPagedProgressEntryAsync(
                    request.PageSize,
                    request.PageNumber,
                    cancellationToken);

                var dtoItems = paged.Items!.Select(ProgressEntryMapper.ToDto);

                return new PagedResult<ProgressEntryDTos>
                {
                    TotalItems = paged.TotalItems,
                    CurrentPage = paged.CurrentPage,
                    TotalPages = paged.TotalPages,
                    Items = dtoItems
                };
            },
            cancellationToken: cancellationToken);

        if (!getPagedProgressEntry.Items!.Any())
        {
            logger.LogError(
                "No progress entries found for the given page request. Page: {PageNumber}, Size: {PageSize}",
                request.PageNumber, request.PageSize);

            return ResultT<PagedResult<ProgressEntryDTos>>.Failure(Error.Failure("400", "The list is empty"));
        }

        logger.LogInformation(
            "Successfully retrieved {TotalItems} progress entries for page {PageNumber} with page size {PageSize}.",
            getPagedProgressEntry.TotalItems, request.PageNumber, request.PageSize);

        return ResultT<PagedResult<ProgressEntryDTos>>.Success(getPagedProgressEntry);
    }
}