using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Events.Commands.Update;

internal sealed class UpdateEventsCommandHandler(
    IEventsRepository eventsRepository,
    ILogger<UpdateEventsCommandHandler> logger)
    : ICommandHandler<UpdateEventsCommand, EventsDto>
{
    public async Task<ResultT<EventsDto>> Handle(
        UpdateEventsCommand request,
        CancellationToken cancellationToken)
    {
        var events = await EntityHelper.GetEntityByIdAsync(eventsRepository.GetByIdAsync,
            request.Id,
            "Events",
            logger);

        if (!events.IsSuccess) return events.Error!;

        events.Value.Description = request.Description;
        events.Value.Title = request.Title;
        await eventsRepository.UpdateAsync(events.Value, cancellationToken);

        logger.LogInformation("Event with ID {EventId} was successfully updated.", request.Id);

        var eventsDto = EventsMapper.EventsToDto(events.Value);

        return ResultT<EventsDto>.Success(eventsDto);
    }
}