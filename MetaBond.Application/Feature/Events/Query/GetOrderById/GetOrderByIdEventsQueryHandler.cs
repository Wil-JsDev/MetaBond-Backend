using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Events.Query.GetOrderById;

internal sealed class GetOrderByIdEventsQueryHandler(
    IEventsRepository eventsRepository,
    IDistributedCache decoratedCache,
    ILogger<GetOrderByIdEventsQueryHandler> logger)
    : IQueryHandler<GetOrderByIdEventsQuery, PagedResult<EventsDto>>
{
    public async Task<ResultT<PagedResult<EventsDto>>> Handle(GetOrderByIdEventsQuery request,
        CancellationToken cancellationToken)
    {
        var paginationValidation = PaginationHelper.ValidatePagination<EventsDto>(request.PageNumber,
            request.PageSize, logger);

        if (!paginationValidation.IsSuccess) return paginationValidation;

        var eventsSort = GetSortValues(request.PageNumber, request.PageSize);
        if (eventsSort.TryGetValue((request.Order ?? string.Empty), out var getSort))
        {
            var eventsListSort = await decoratedCache.GetOrCreateAsync(
                $"order-{request.Order}",
                async () =>
                {
                    var eventSort = await getSort(cancellationToken);
                    IEnumerable<EventsDto> eventsDtos = eventSort.Items.Select(EventsMapper.EventsToDto).ToList();

                    PagedResult<EventsDto> pagedResult = new(
                        currentPage: eventSort.CurrentPage,
                        items: eventsDtos,
                        totalItems: eventSort.TotalItems,
                        pageSize: request.PageSize
                    );

                    return pagedResult;
                },
                cancellationToken: cancellationToken);

            if (!eventsListSort.Items.Any())
            {
                logger.LogError("No events found for the specified order.");

                return ResultT<PagedResult<EventsDto>>.Failure(Error.Failure("400",
                    "No events found for the given order."));
            }

            logger.LogInformation("Successfully retrieved {Count} events ordered by {Order}.",
                eventsListSort.Items.Count(),
                request.Order);

            return ResultT<PagedResult<EventsDto>>.Success(eventsListSort);
        }

        logger.LogError("Invalid order parameter received: {Order}. Please specify 'asc' or 'desc'.",
            request.Order);

        return ResultT<PagedResult<EventsDto>>.Failure(Error.Failure("400",
            "Invalid order parameter. Please specify 'asc' or 'desc'."));
    }

    #region Private Methods

    private Dictionary<string, Func<CancellationToken, Task<PagedResult<Domain.Models.Events>>>> GetSortValues(
        int pageNumber, int pageSize)
    {
        return new Dictionary<string, Func<CancellationToken, Task<PagedResult<Domain.Models.Events>>>>
        {
            {
                "asc", async cancellationToken => await eventsRepository.GetOrderByIdAscAsync(pageNumber, pageSize,
                    cancellationToken)
            },
            {
                "desc", async cancellationToken => await eventsRepository.GetOrderByIdDescAsync(pageNumber, pageSize,
                    cancellationToken)
            }
        };
    }

    #endregion
}