using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Application.Feature.Events.Query.GetOrderById
{
    public sealed class GetOrderByIdEventsQuery : IQuery<IEnumerable<EventsDto>>
    {
        public string? Order {  get; set; } 
    }
}
