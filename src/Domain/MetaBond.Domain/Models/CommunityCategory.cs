using MetaBond.Domain.Common;

namespace MetaBond.Domain.Models;

public sealed class CommunityCategory : BaseModel
{
    public DateTime CreateAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdateAt { get; set; }

    public ICollection<Communities>? Communities { get; set; }
}