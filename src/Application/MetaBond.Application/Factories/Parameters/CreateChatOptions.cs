using MetaBond.Domain;

namespace MetaBond.Application.Factories.Parameters;

public sealed class CreateChatOptions
{
    public ChatType Type { get; set; }
    public Guid? CommunityId { get; set; }
    public string? PhotoUrl { get; set; }
    public string? Name { get; set; }
}