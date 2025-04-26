using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Domain.Models;

    public sealed class Posts
    {
        public Guid Id { get; set; }
        
        public string? Title { get; set; }
        
        public string? Content { get; set; }
        
        public string? Image {  get; set; }
        
        public Guid? CommunitiesId { get; set; }
    
        public Communities? Communities { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid CreatedById { get; set; }
        
        public User? CreatedBy { get; set; }
    }

