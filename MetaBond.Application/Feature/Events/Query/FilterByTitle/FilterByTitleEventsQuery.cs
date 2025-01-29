using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;

namespace MetaBond.Application.Feature.Events.Query.FilterByTitle
{
    public sealed class FilterByTitleEventsQuery : IQuery<IEnumerable<EventsDto>>
    {
        public string? Title { get; set; }
    }
}
