using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;

namespace MetaBond.Application.Feature.Events.Query.GetOrderById
{
    public sealed class GetOrderByIdEventsQuery : IQuery<IEnumerable<EventsDto>>
    {
        public string? Order {  get; set; } 
    }
}
