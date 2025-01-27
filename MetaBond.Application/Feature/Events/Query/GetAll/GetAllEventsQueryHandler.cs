using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Application.Feature.Events.Query.GetAll
{
    internal sealed class GetAllEventsQueryHandler : IQueryHandler<GetAllEventsQuery, IEnumerable<EventsDto>>
    {
        private readonly IEventsRepository _eventsRepository;

        public GetAllEventsQueryHandler(IEventsRepository eventsRepository)
        {
            _eventsRepository = eventsRepository;
        }

        public async Task<ResultT<IEnumerable<EventsDto>>> Handle(GetAllEventsQuery request, CancellationToken cancellationToken)
        {
            var events = await _eventsRepository.GetAll(cancellationToken);
            if (events != null)
            {
                IEnumerable<EventsDto> eventsDtos = events.Select(x => new EventsDto 
                (
                    Id: x.Id,
                    Description: x.Description,
                    Title: x.Title,
                    DateAndTime: x.DateAndTime,
                    CreatedAt: x.CreateAt,
                    CommunitiesId: x.CommunitiesId,
                    ParticipationInEventId: x.ParticipationInEventId
                ));

                return ResultT<IEnumerable<EventsDto>>.Success(eventsDtos);
            }
           return ResultT<IEnumerable< EventsDto >>.Failure(Error.Failure("400", "No events found. Please try again later."));
        }
    }
}
