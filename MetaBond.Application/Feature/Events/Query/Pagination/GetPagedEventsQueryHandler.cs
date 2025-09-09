using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Events.Query.Pagination;

internal sealed class GetPagedEventsQueryHandler(
    IEventsRepository eventsRepository,
    IDistributedCache decoratedCache,
    ILogger<GetPagedEventsQueryHandler> logger)
    : IQueryHandler<GetPagedEventsQuery, PagedResult<EventsDto>>
{
    public async Task<ResultT<PagedResult<EventsDto>>> Handle(GetPagedEventsQuery request,
        CancellationToken cancellationToken)
    {
        var validationPagination = PaginationHelper.ValidatePagination<EventsDto>(request.PageNumber,
            request.PageSize, logger);

        if (!validationPagination.IsSuccess)
            return validationPagination;

        string cacheKey = $"paged-events-page-{request.PageNumber}-size-{request.PageSize}";
        var eventsPaged = await decoratedCache.GetOrCreateAsync(
            cacheKey,
            async () =>
            {
                var pagedEvent = await eventsRepository.GetPagedEventsAsync(request.PageNumber, request.PageSize,
                    cancellationToken);

                var dtoItems = pagedEvent.Items!.Select(EventsMapper.EventsToDto).ToList();

                PagedResult<EventsDto> result = new()
                {
                    TotalItems = pagedEvent.TotalItems,
                    CurrentPage = pagedEvent.CurrentPage,
                    TotalPages = pagedEvent.TotalPages,
                    Items = dtoItems
                };

                return result;
            },
            cancellationToken: cancellationToken);

        if (!eventsPaged.Items!.Any())
        {
            logger.LogError("No events found for the given page number and page size.");

            return ResultT<PagedResult<EventsDto>>.Failure(Error.NotFound("404",
                "No events found for the given page number and page size."));
        }

        logger.LogInformation("Paged events retrieved successfully. Page {PageNumber} of {TotalPages}.",
            request.PageNumber, eventsPaged.TotalPages);

        return ResultT<PagedResult<EventsDto>>.Success(eventsPaged);
    }
}