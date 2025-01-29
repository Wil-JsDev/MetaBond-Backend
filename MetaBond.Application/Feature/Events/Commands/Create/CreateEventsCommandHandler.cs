using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using MetaBond.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Application.Feature.Events.Commands.Create
{
    internal sealed class CreateEventsCommandHandler : ICommandHandler<CreateEventsCommand, EventsDto>
    {
        private readonly IEventsRepository _eventsRepository;

        public CreateEventsCommandHandler(IEventsRepository eventsRepository)
        {
            _eventsRepository = eventsRepository;
        }

        public async Task<ResultT<EventsDto>> Handle(CreateEventsCommand request, CancellationToken cancellationToken)
        {
            Domain.Models.Events events = new()
            {
                Id = Guid.NewGuid(),
                Description = request.Description,
                Title = request.Title,  
                DateAndTime = request.DateAndTime,
                CommunitiesId = request.CommunitiesId,
                ParticipationInEventId = request.ParticipationInEventId,
            };

            if (events != null)
            {
                await _eventsRepository.CreateAsync(events,cancellationToken);
                EventsDto eventsDto = new 
                (
                   Id: events.Id,
                   Description: events.Description,
                   Title: events.Title,
                   DateAndTime: events.DateAndTime,
                   CreatedAt: events.CreateAt,
                   CommunitiesId: events.CommunitiesId,
                   ParticipationInEventId: events.ParticipationInEventId
                );

                return ResultT<EventsDto>.Success(eventsDto);
            }

            return ResultT<EventsDto>.Failure(Error.Failure("400", "Failed to create the event. Please check the provided data."));
        }
    }
}
