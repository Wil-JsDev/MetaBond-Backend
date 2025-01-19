﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Domain.Models
{
    public sealed class ProgressEntry
    {
        public Guid Id { get; set; } 

        public Guid CommunitiesId { get; set; } 

        public Communities? Communities { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdateAt {  get; set; } 
    }
}