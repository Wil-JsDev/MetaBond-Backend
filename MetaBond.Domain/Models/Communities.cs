﻿using MetaBond.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MetaBond.Domain.Models;

    public sealed class Communities : BaseModel
    {
        public string? Description { get; set; }

        public string? Category { get; set; }

        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
    
        public ProgressBoard? ProgressBoard { get; set; }
    
        public ICollection<Posts>? Posts { get; set; }
        
        public ICollection<CommunityUser>? CommunityUsers { get; set; } = new List<CommunityUser>();
        
        public ICollection<CommunityManager>? CommunityManagers { get; set; } = new List<CommunityManager>();
        
        public ICollection<ModeratorCommunity>? ModeratorCommunities { get; set; } = new List<ModeratorCommunity>();
        
        [JsonIgnore]
        public ICollection<Events>? Events { get; set; }
    }

