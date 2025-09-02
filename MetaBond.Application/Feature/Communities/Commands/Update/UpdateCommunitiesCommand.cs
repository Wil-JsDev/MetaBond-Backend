using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communities;

namespace MetaBond.Application.Feature.Communities.Commands.Update;

public class UpdateCommunitiesCommand : ICommand<CommunitiesDTos>
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Category { get; set; }
    
    public string? Description { get; set; }
}