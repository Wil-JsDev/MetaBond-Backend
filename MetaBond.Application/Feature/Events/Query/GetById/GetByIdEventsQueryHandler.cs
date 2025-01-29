using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Application.Feature.Events.Query.GetById
{
    internal sealed class GetByIdEventsQueryHandler : IQueryHandler<GetByIdEventsQuery, EventsDto>
    {
        private readonly IEventsRepository _eventsRepository;

        public GetByIdEventsQueryHandler(IEventsRepository eventsRepository)
        {
            _eventsRepository = eventsRepository;
        }

        public async Task<ResultT<EventsDto>> Handle(GetByIdEventsQuery request, CancellationToken cancellationToken)
        {
            var events = await _eventsRepository.GetByIdAsync(request.Id);
            if (events != null)
            {
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

            return ResultT<EventsDto>.Failure(Error.NotFound("404", $"{request.Id} not found"));
        }
    }
}
