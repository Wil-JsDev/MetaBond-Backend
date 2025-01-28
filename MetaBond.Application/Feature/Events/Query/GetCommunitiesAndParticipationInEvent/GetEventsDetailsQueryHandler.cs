using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communties;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Feature.Events.Query.GetCommunitiesAndParticipationInEvent
{
    internal sealed class GetEventsDetailsQueryHandler : IQueryHandler<GetEventsDetailsQuery, IEnumerable<CommunitiesAndParticipationInEventDTos>>
    {
        private readonly IEventsRepository _eventsRepository;

        public GetEventsDetailsQueryHandler(IEventsRepository eventsRepository)
        {
            _eventsRepository = eventsRepository;
        }

        public async Task<ResultT<IEnumerable<CommunitiesAndParticipationInEventDTos>>> Handle(GetEventsDetailsQuery request, CancellationToken cancellationToken)
        {
            var events = await _eventsRepository.GetByIdAsync(request.Id);
            if (events == null)
            {
                return ResultT<IEnumerable<CommunitiesAndParticipationInEventDTos>>.Failure(Error.NotFound("404", $"{request.Id} not found"));
            }

            var evenntsDetails = await _eventsRepository.GetCommunitiesAndParticipationInEvent(request.Id,cancellationToken);
            if (!evenntsDetails.Any())
            {
                return ResultT<IEnumerable<CommunitiesAndParticipationInEventDTos>>.Failure(Error.NotFound("404", "No communities or participation in event found for this events."));
            }
           
                IEnumerable<CommunitiesAndParticipationInEventDTos> inEventDTos = evenntsDetails.Select(e => new CommunitiesAndParticipationInEventDTos
                (
                    Id: e.Id,
                    Description: e.Description,
                    Title: e.Title,
                    DateAndTime: e.DateAndTime,
                    CreatedAt: e.CreateAt,
                    Communities: e.Communities ?? new Domain.Models.Communities(),
                    ParticipationInEvent: e.ParticipationInEvent ?? new List<Domain.Models.ParticipationInEvent>()
                ));

                return ResultT<IEnumerable<CommunitiesAndParticipationInEventDTos>>.Success(inEventDTos); 
        }
    }
}
