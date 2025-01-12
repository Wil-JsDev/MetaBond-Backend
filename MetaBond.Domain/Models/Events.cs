using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Domain.Models;

    public sealed class Events
    {
        public Guid? Id { get; set; }

        public string? Title {  get; set; }    
        
        public DateTime? DateAndTime { get; set; }

        public DateTime? CreateAt { get; set; } = DateTime.UtcNow;

        public Guid? CommunitiesId { get; set; }

        public Communities? Communities { get; set; }

        public ICollection<ParticipationInEvent>? ParticipationInEvent { get; set; }
    }

