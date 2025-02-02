using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ParticipationInEventDtos;

namespace MetaBond.Application.Feature.ParticipationInEvent.Querys.GetEvent
{
    public sealed class GetEventParticipationInEventIdQuery : IQuery<IEnumerable<ParticipationInEventWithEventsDTos>>
    {
        public Guid EventId { get; set; }
    }
}
