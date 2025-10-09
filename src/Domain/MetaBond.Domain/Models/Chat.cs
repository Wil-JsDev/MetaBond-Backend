using MetaBond.Domain.Common;

namespace MetaBond.Domain.Models;

public sealed class Chat : BaseModel
{
    public string? Type { get; set; } = ChatType.Direct.ToString();

    //Only group
    public Guid? CommunityId { get; set; }
    public Communities? Community { get; set; }

    public string? Photo { get; set; }

    // The last message
    public string? LastMessage { get; set; }
    public DateTime? LastMessageAt { get; set; }

    // Relationships
    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public ICollection<UserChat> UserChats { get; set; } = new List<UserChat>();
}