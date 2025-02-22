
using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;

namespace MetaBond.Application.Feature.Events.Query.GetParticipationInEvent
{
    public sealed class GetParticipationInEventQuery : IQuery<IEnumerable<EventsWithParticipationInEventsDTos>>
    {
        public Guid? EventsId { get; set; }
    }
}
