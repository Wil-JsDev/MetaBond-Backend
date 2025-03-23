using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communties;

namespace MetaBond.Application.Feature.Communities.Commands.Create;

public sealed class CreateCommunitiesCommand : ICommand<CommunitiesDTos> 
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
}