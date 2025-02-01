using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communties;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using MetaBond.Domain.Models;
using Microsoft.Extensions.Logging;
using Serilog.Core;

namespace MetaBond.Application.Feature.Events.Query.GetCommunitiesAndParticipationInEvent
{
    internal sealed class GetEventsDetailsQueryHandler : IQueryHandler<GetEventsDetailsQuery, IEnumerable<CommunitiesAndParticipationInEventDTos>>
    {
        private readonly IEventsRepository _eventsRepository;
        private readonly ILogger<GetEventsDetailsQueryHandler> _logger;

        public GetEventsDetailsQueryHandler(
            IEventsRepository eventsRepository, 
            ILogger<GetEventsDetailsQueryHandler> logger)
        {
            _eventsRepository = eventsRepository;
            _logger = logger;
        }

        public async Task<ResultT<IEnumerable<CommunitiesAndParticipationInEventDTos>>> Handle(
            GetEventsDetailsQuery request, 
            CancellationToken cancellationToken)
        {
            var events = await _eventsRepository.GetByIdAsync(request.Id);
            if (events == null)
            {
                _logger.LogError("Event with ID {Id} not found.", request.Id);

                return ResultT<IEnumerable<CommunitiesAndParticipationInEventDTos>>.Failure(Error.NotFound("404", $"{request.Id} not found"));
            }

            var evenntsDetails = await _eventsRepository.GetCommunitiesAndParticipationInEvent(request.Id,cancellationToken);
            if (!evenntsDetails.Any())
            {
                _logger.LogError("No communities or participation in event found for event with ID {Id}.", request.Id);

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

            _logger.LogInformation("Event details with ID {Id} retrieved successfully.", request.Id);

            return ResultT<IEnumerable<CommunitiesAndParticipationInEventDTos>>.Success(inEventDTos);
        }
    }
}
