using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Events.Query.FilterByDateRange;

internal sealed class FilterByDateRangeEventsQueryHandler(
    IEventsRepository eventsRepository,
    IDistributedCache decoratedCache,
    ILogger<FilterByDateRangeEventsQueryHandler> logger)
    : IQueryHandler<FilterByDateRangeEventsQuery, IEnumerable<EventsDto>>
{
    public async Task<ResultT<IEnumerable<EventsDto>>> Handle(
        FilterByDateRangeEventsQuery request,
        CancellationToken cancellationToken)
    {
        var exists = await eventsRepository.ValidateAsync(x => x.Id == request.CommunitiesId, cancellationToken);
        if (!exists)
        {
            logger.LogError("The community with ID {Id} does not exist.", request.CommunitiesId);

            return ResultT<IEnumerable<EventsDto>>.Failure(Error.NotFound("404",
                $"{request.CommunitiesId} not found"));
        }

        var status = FilterByDateRange(request.CommunitiesId);
        if (status.TryGetValue((request.DateRangeFilter), out var getStatusList))
        {
            string cacheKey = $"events-community-{request.CommunitiesId}-filter-{request.DateRangeFilter}";
            var result = await decoratedCache.GetOrCreateAsync(
                cacheKey,
                async () =>
                {
                    var eventsLists = await getStatusList(cancellationToken);

                    var listsEvents = eventsLists.ToList();
                    IEnumerable<EventsDto> dTos = listsEvents.Select(EventsMapper.EventsToDto).ToList();

                    return dTos;
                },
                cancellationToken: cancellationToken);

            var eventsDtos = result.ToList();
            if (!eventsDtos.Any())
            {
                var valuesFilter = GetValueFilter().TryGetValue((request.DateRangeFilter), out var values);
                if (valuesFilter)
                {
                    logger.LogInformation("No events found for the given filter");

                    return ResultT<IEnumerable<EventsDto>>.Failure(Error.NotFound("404", values!));
                }

                logger.LogError("The events list is empty due to an unknown error.");

                return ResultT<IEnumerable<EventsDto>>.Failure(Error.Failure("400", "The list is empty"));
            }

            logger.LogInformation("Events retrieved successfully");

            return ResultT<IEnumerable<EventsDto>>.Success(eventsDtos);
        }

        logger.LogError("No data entered for the status.");

        return ResultT<IEnumerable<EventsDto>>.Failure(Error.Failure("400", "No data entered"));
    }

    # region Private Methods

    private Dictionary<Domain.DateRangeFilter, Func<CancellationToken, Task<IEnumerable<Domain.Models.Events>>>>
        FilterByDateRange(Guid communitiesId)
    {
        return new Dictionary<Domain.DateRangeFilter,
            Func<CancellationToken, Task<IEnumerable<Domain.Models.Events>>>>
        {
            {
                DateRangeFilter.LastDay,
                async cancellationToken => await eventsRepository.FilterByDateRange(communitiesId,
                    DateTime.UtcNow.AddDays(-1), cancellationToken)
            },
            {
                DateRangeFilter.ThreeDays,
                async cancellationToken => await eventsRepository.FilterByDateRange(communitiesId,
                    DateTime.UtcNow.AddDays(-3), cancellationToken)
            },
            {
                DateRangeFilter.LastWeek,
                async cancellationToken => await eventsRepository.FilterByDateRange(communitiesId,
                    DateTime.UtcNow.AddDays(-7), cancellationToken)
            }
        };
    }

    private static Dictionary<Domain.DateRangeFilter, string> GetValueFilter()
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