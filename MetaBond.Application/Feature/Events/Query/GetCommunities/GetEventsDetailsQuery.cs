using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;

namespace MetaBond.Application.Feature.Events.Query.GetCommunitiesAndParticipationInEvent
{
    public sealed class GetEventsDetailsQuery : IQuery<IEnumerable<CommunitiesDTos>>
    {
        public Guid Id { get; set; }
    }
}
