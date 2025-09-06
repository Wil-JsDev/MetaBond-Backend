using MetaBond.Domain.Common;
using System.Text.Json.Serialization;

namespace MetaBond.Domain.Models;

public sealed class Communities : BaseModel
{
    public string? Description { get; set; }

    public Guid? CommunityCategoryId { get; set; }

    public DateTime CreateAt { get; set; } = DateTime.UtcNow;

    public ProgressBoard? ProgressBoard { get; set; }

    public ICollection<Posts>? Posts { get; set; }

    public ICollection<CommunityMembership>? CommunityMemberships { get; set; }

    public CommunityCategory? CommunityCategory { get; set; }

    [JsonIgnore] public ICollection<Events>? Events { get; set; }
}