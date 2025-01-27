using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MetaBond.Application.Feature.Events.Commands.Update
{
    public sealed class UpdateEventsCommand : ICommand<EventsDto>
    {
        public Guid Id { get; set; }

        public string? Description { get; set; }

        public string? Title { get; set; }

        public Guid ParticipationInEventId { get; set; }

    }
}
