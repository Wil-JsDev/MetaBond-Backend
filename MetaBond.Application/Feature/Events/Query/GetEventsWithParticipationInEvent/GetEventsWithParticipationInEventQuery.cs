
using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;

namespace MetaBond.Application.Feature.Events.Query.GetEventsWithParticipationInEvent
{
    public sealed class GetEventsWithParticipationInEventQuery : IQuery<EventsWithParticipationInEventsDTos>
    {
        public Guid? EventsId { get; set; }
    }
}
