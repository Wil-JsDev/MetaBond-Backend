using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.CommunityCategory;

namespace MetaBond.Application.Feature.CommunityCategory.Command.Update;

public sealed class UpdateCommunityCategoryCommand : ICommand<CommunityCategoryDTos>
{
    public Guid CommunityCategoryId { get; set; }

    public string? Name { get; set; }
}