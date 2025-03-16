using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Events.Commands.Update
{
    internal sealed class UpdateEventsCommandHandler(
        IEventsRepository eventsRepository,
        ILogger<UpdateEventsCommandHandler> logger)
        : ICommandHandler<UpdateEventsCommand, EventsDto>
    {
        public async Task<ResultT<EventsDto>> Handle(
            UpdateEventsCommand request, 
            CancellationToken cancellationToken)
        {
            var events = await eventsRepository.GetByIdAsync(request.Id);
            if (events != null)
            {
                events.Description = request.Description;
                events.Title = request.Title;
                await eventsRepository.UpdateAsync(events, cancellationToken);

                logger.LogInformation("Event with ID {EventId} was successfully updated.", request.Id);

                EventsDto eventsDto = new
                (
                    Id: events.Id,
                    Description: events.Description,
                    Title: events.Title,
                    DateAndTime: events.DateAndTime,
                    CreatedAt: events.CreateAt,
                    CommunitiesId: events.CommunitiesId
                );

                return ResultT<EventsDto>.Success(eventsDto);
            }

            logger.LogError("Failed to update event. Event with ID {EventId} was not found.", request.Id);

            return ResultT<EventsDto>.Failure(Error.NotFound("404", $"{request.Id} not found"));
            
        }
    }
}
