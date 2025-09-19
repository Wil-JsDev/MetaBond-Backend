using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Events.Query.GetEventsWithParticipationInEvent;

internal sealed class GetEventsWithParticipationInEventQueryHandler(
    IEventsRepository eventsRepository,
    IDistributedCache decoratedCache,
    ILogger<GetEventsWithParticipationInEventQueryHandler> logger)
    : IQueryHandler<GetEventsWithParticipationInEventQuery, PagedResult<EventsWithParticipationInEventsDTos>>
{
    public async Task<ResultT<PagedResult<EventsWithParticipationInEventsDTos>>> Handle(
        GetEventsWithParticipationInEventQuery request,
        CancellationToken cancellationToken)
    {
        var events = await EntityHelper.GetEntityByIdAsync
        (
            eventsRepository.GetByIdAsync,
            request.EventsId ?? Guid.Empty,
            "Events",
            logger
        );
        if (!events.IsSuccess) return events.Error!;

        var eventsWithParticipationInEvents = await decoratedCache.GetOrCreateAsync(
            $"eventsId-{request.EventsId}-participation-{request.PageNumber}-{request.PageSize}",
            async () =>
            {
                var eventsList =
                    await eventsRepository.GetEventsWithParticipationAsync(events.Value.Id,
                        request.PageNumber,
                        request.PageSize,
                        cancellationToken);

                var eventsEnumerable = eventsList.Items.ToList();

                PagedResult<EventsWithParticipationInEventsDTos> pagedResult = new(
                    currentPage: eventsList.CurrentPage,
                    items: eventsEnumerable.ToEventsWithParticipationDtos(),
                    totalItems: eventsList.TotalItems,
                    pageSize: request.PageSize
                );

                return pagedResult;
            },
            cancellationToken: cancellationToken);

        if (eventsWithParticipationInEvents.Items != null && !eventsWithParticipationInEvents.Items.Any())
        {
            logger.LogWarning("No events found with participation. Returning an empty result.");

            return ResultT<PagedResult<EventsWithParticipationInEventsDTos>>.Failure(
                Error.NotFound("404", "No events with participation were found.")
            );
        }

        if (eventsWithParticipationInEvents.Items != null)
            logger.LogInformation("Successfully retrieved {Count} participation for Event ID: {EventId}",
                eventsWithParticipationInEvents.Items.Count(),
                eventsWithParticipationInEvents.Items.FirstOrDefault()?.EventsId ?? Guid.Empty);

        return ResultT<PagedResult<EventsWithParticipationInEventsDTos>>.Success(eventsWithParticipationInEvents);
    }
}