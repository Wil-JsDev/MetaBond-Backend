using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Domain.Models;

    public sealed class ParticipationInEvent
    {
        public Guid Id { get; set; }
        
        public Guid? EventId { get; set; }

        public ICollection<EventParticipation>? EventParticipations { get; set; }

    }

