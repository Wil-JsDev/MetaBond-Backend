using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ParticipationInEvent;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ParticipationInEvent.Querys.GetAll
{
    internal sealed class GetAllParticipationInEventQueryHandler : IQueryHandler<GetAllParticipationInEventQuery, IEnumerable<ParticipationInEventDTos>>
    {

        private readonly IParticipationInEventRepository _participationInEventRepository;
        private readonly ILogger<GetAllParticipationInEventQueryHandler> _logger;

        public GetAllParticipationInEventQueryHandler(
            IParticipationInEventRepository participationInEventRepository, 
            ILogger<GetAllParticipationInEventQueryHandler> logger)
        {
            _participationInEventRepository = participationInEventRepository;
            _logger = logger;
        }

        public async Task<ResultT<IEnumerable<ParticipationInEventDTos>>> Handle(
            GetAllParticipationInEventQuery request, 
            CancellationToken cancellationToken)
        {

            var participationInEvent = await _participationInEventRepository.GetAll(cancellationToken);

            if (participationInEvent != null && participationInEvent.Any())
            {
                IEnumerable<ParticipationInEventDTos> inEventDTos = participationInEvent.Select(x => new ParticipationInEventDTos
                (
                    ParticipationInEventId: x.Id,
                    EventId: x.EventId
                ));

                _logger.LogInformation("Successfully retrieved {ParticipationCount} participations.", participationInEvent.Count());

                return ResultT<IEnumerable<ParticipationInEventDTos>>.Success(inEventDTos);
            }

            _logger.LogError("No participation found for the query.");

            return ResultT<IEnumerable<ParticipationInEventDTos>>.Failure(Error.Failure("404", "No participations found."));
        }
    }
}
