using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;

namespace MetaBond.Application.Feature.Events.Query.FilterByTitleCommunityId;

public class GetEventsByTitleAndCommunityIdQuery : IQuery<IEnumerable<EventsDto>>
{
    public Guid CommunitiesId { get; set; }
    
    public string? Title { get; set; }
}