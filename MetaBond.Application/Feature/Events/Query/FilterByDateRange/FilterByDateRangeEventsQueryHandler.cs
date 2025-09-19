using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Events.Query.FilterByDateRange;

internal sealed class FilterByDateRangeEventsQueryHandler(
    IEventsRepository eventsRepository,
    ICommunitiesRepository communitiesRepository,
    IDistributedCache decoratedCache,
    ILogger<FilterByDateRangeEventsQueryHandler> logger)
    : IQueryHandler<FilterByDateRangeEventsQuery, PagedResult<EventsDto>>
{
    public async Task<ResultT<PagedResult<EventsDto>>> Handle(
        FilterByDateRangeEventsQuery request,
        CancellationToken cancellationToken)
    {
        var exists = await communitiesRepository.ValidateAsync(x => x.Id == request.CommunitiesId, cancellationToken);
        if (!exists)
        {
            logger.LogError("The community with ID {Id} does not exist.", request.CommunitiesId);

            return ResultT<PagedResult<EventsDto>>.Failure(Error.NotFound("404",
                $"{request.CommunitiesId} not found"));
        }

        var validationPagination =
            PaginationHelper.ValidatePagination<EventsDto>(request.PageNumber, request.PageSize, logger);

        if (!validationPagination.IsSuccess) return validationPagination;

        var status = FilterByDateRange(request.CommunitiesId, request.PageNumber, request.PageSize);
        if (status.TryGetValue((request.DateRangeFilter), out var getStatusList))
        {
            string cacheKey = $"events-community-{request.CommunitiesId}-filter-{request.DateRangeFilter}";
            var result = await decoratedCache.GetOrCreateAsync(
                cacheKey,
                async () =>
                {
                    var eventsLists = await getStatusList(cancellationToken);

                    var listsEvents = eventsLists.Items!.ToList();

                    IEnumerable<EventsDto> dTos = listsEvents.Select(EventsMapper.EventsToDto).ToList();

                    PagedResult<EventsDto> pagedResult = new(
                        currentPage: eventsLists.CurrentPage,
                        items: dTos,
                        totalItems: eventsLists.TotalItems,
                        pageSize: request.PageSize
                    );

                    return pagedResult;
                },
                cancellationToken: cancellationToken);

            if (!result.Items!.Any())
            {
                var valuesFilter = GetValueFilter().TryGetValue((request.DateRangeFilter), out var values);
                if (valuesFilter)
                {
                    logger.LogInformation("No events found for the given filter");

                    return ResultT<PagedResult<EventsDto>>.Failure(Error.NotFound("404", values!));
                }

                logger.LogError("The events list is empty due to an unknown error.");

                return ResultT<PagedResult<EventsDto>>.Failure(Error.Failure("400", "The list is empty"));
            }

            logger.LogInformation("Events retrieved successfully");

            return ResultT<PagedResult<EventsDto>>.Success(result);
        }

        logger.LogError("No data entered for the status.");

        return ResultT<PagedResult<EventsDto>>.Failure(Error.Failure("400", "No data entered"));
    }

    # region Private Methods

    private Dictionary<DateRangeFilter, Func<CancellationToken, Task<PagedResult<Domain.Models.Events>>>>
        FilterByDateRange(Guid communitiesId, int pageNumber, int pageSize)
    {
        return new Dictionary<DateRangeFilter,
            Func<CancellationToken, Task<PagedResult<Domain.Models.Events>>>>
        {
            {
                DateRangeFilter.LastDay,
                async cancellationToken => await eventsRepository.FilterByDateRangeAsync(
                    communitiesId,
                    DateTime.UtcNow.AddDays(-1),
                    pageNumber,
                    pageSize,
                    cancellationToken)
            },
            {
                DateRangeFilter.ThreeDays,
                async cancellationToken => await eventsRepository.FilterByDateRangeAsync(
                    communitiesId,
                    DateTime.UtcNow.AddDays(-3),
                    pageNumber,
                    pageSize,
                    cancellationToken)
            },
            {
                DateRangeFilter.LastWeek,
                async cancellationToken => await eventsRepository.FilterByDateRangeAsync(
                    communitiesId,
                    DateTime.UtcNow.AddDays(-7),
                    pageNumber,
                    pageSize,
                    cancellationToken)
            }
        };
    }


    private static Dictionary<DateRangeFilter, string> GetValueFilter()
    {
        return new Dictionary<DateRangeFilter, string>
        {
            { DateRangeFilter.LastDay, "No events found for the last day filter." },
            { DateRangeFilter.ThreeDays, "No events found for the last 3 days filter." },
            { DateRangeFilter.LastWeek, "No events found for the last week filter." }
        };
    }

    # endregion
}