using MetaBond.Application.Abstractions.Messaging;

namespace MetaBond.Application.Feature.Communities.Commands.Delete;

public class DeleteCommunitiesCommand : ICommand<Guid>
{
    public Guid Id { get; set; }
}