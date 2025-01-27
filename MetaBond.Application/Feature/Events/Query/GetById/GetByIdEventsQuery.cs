using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Application.Feature.Events.Query.GetById
{
    public sealed class GetByIdEventsQuery : IQuery<EventsDto>
    {
        public Guid Id { get; set; }
    }
}
