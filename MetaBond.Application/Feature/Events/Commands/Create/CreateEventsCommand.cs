using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Application.Feature.Events.Commands.Create
{
    public sealed class CreateEventsCommand : ICommand<EventsDto>
    {
        public string? Description { get; set; }

        public string? Title { get; set; }

        public DateTime? DateAndTime { get; set; }

        public Guid? CommunitiesId { get; set; }
    }
}
