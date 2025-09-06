using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Events.Query.GetById;

internal sealed class GetByIdEventsQueryHandler(
    IEventsRepository eventsRepository,
    IDistributedCache decoratedCache,
    ILogger<GetByIdEventsQueryHandler> logger)
    : IQueryHandler<GetByIdEventsQuery, EventsDto>
{
    public async Task<ResultT<EventsDto>> Handle(
        GetByIdEventsQuery request,
        CancellationToken cancellationToken)
    {
        var events = await EntityHelper.GetEntityByIdAsync
        (
            eventsRepository.GetByIdAsync,
            request.Id,
            "Events",
            logger
        );
        if (!events.IsSuccess)
        {
            logger.LogError("Events with ID {EventId} not found.", request.Id);

            return ResultT<EventsDto>.Failure(events.Error!);
        }

        var eventsDto = EventsMapper.EventsToDto(events.Value);

        logger.LogInformation("Event with ID {Id} retrieved successfully.", events.Value.Id);

        return ResultT<EventsDto>.Success(eventsDto);
    }
}