using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Events.Commands.Create
{
    internal sealed class CreateEventsCommandHandler(
        IEventsRepository eventsRepository,
        ILogger<CreateEventsCommandHandler> logger)
        : ICommandHandler<CreateEventsCommand, EventsDto>
    {
        public async Task<ResultT<EventsDto>> Handle(
            CreateEventsCommand request, 
            CancellationToken cancellationToken)
        {

            if (request != null)
            {
                Domain.Models.Events events = new()
                {
                    Id = Guid.NewGuid(),
                    Description = request.Description,
                    Title = request.Title,  
                    DateAndTime = request.DateAndTime,
                    CommunitiesId = request.CommunitiesId
                };

                await eventsRepository.CreateAsync(events,cancellationToken);

                logger.LogInformation("Community {CommunityId} created successfully.", events.Id);

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

            logger.LogError("An error occurred while creating the event. Request: {@Request}", request);

            return ResultT<EventsDto>.Failure(Error.Failure("400", "Failed to create the event. Please check the provided data."));

        }
    }
}
