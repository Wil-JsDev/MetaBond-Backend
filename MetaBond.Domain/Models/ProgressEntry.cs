using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Domain.Models
{
    public sealed class ProgressEntry
    {
        public Guid Id { get; set; }

        public Guid ProgressBoardId { get; set; }

        public ProgressBoard? ProgressBoard { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdateAt { get; set; }

        public Guid UserId { get; set; }

        public User? User { get; set; }
    }
}