using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Application.Feature.Events.Query.FilterByTitle
{
    internal sealed class FilterByTitleEventsQueryHandler : IQueryHandler<FilterByTitleEventsQuery, IEnumerable<EventsDto>>
    {
        private readonly IEventsRepository _eventsRepository;

        public FilterByTitleEventsQueryHandler(IEventsRepository eventsRepository)
        {
            _eventsRepository = eventsRepository;
        }

        public async Task<ResultT<IEnumerable<EventsDto>>> Handle(FilterByTitleEventsQuery request, CancellationToken cancellationToken)
        {
            if (request.Title != null)
            {
                var eventsTitle = await _eventsRepository.GetFilterByTitleAsync(request.Title, cancellationToken);
                IEnumerable<EventsDto> eventsDtos = eventsTitle.Select(c => new EventsDto 
                (
                    Id: c.Id,
                    Description: c.Description,
                    Title: c.Title,
                    DateAndTime: c.DateAndTime,
                    CreatedAt: c.CreateAt,
                    CommunitiesId: c.CommunitiesId,
                    ParticipationInEventId: c.ParticipationInEventId 
                ));
                return ResultT<IEnumerable<EventsDto>>.Success(eventsDtos);
            }

            return ResultT<IEnumerable<EventsDto>>.Failure(Error.Failure("400",$"{request.Title} not found"));
        }
    }
}
