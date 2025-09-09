using MetaBond.Domain.Common;

namespace MetaBond.Domain.Models;

public sealed class InterestCategory : BaseModel
{
    public DateTime CreateAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdateAt { get; set; }

    public ICollection<Interest>? Interest { get; set; }
}