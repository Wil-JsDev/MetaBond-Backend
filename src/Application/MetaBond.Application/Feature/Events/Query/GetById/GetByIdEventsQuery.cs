using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;

namespace MetaBond.Application.Feature.Events.Query.GetById;

public sealed class GetByIdEventsQuery : IQuery<EventsDto>
{
    public Guid Id { get; set; }
}