using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.CommunityCategory;

namespace MetaBond.Application.Feature.CommunityCategory.Command.Create;

public sealed class CreateCommunityCategoryCommand : ICommand<CommunityCategoryDTos>
{
    public string? Name { get; set; }
}