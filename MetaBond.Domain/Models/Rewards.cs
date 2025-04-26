using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Domain.Models;

    public sealed class Rewards
    {
        public Guid Id { get; set; }

        public string? Description { get; set; }

        public int? PointAwarded { get; set; }
        
        public DateTime? DateAwarded { get; set; }
        
        public Guid UserId { get; set; }
        
        public User? User { get; set; }
    }

