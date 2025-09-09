using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Interest;

namespace MetaBond.Application.Feature.Interest.Commands.Create;

public sealed class CreateInterestCommand : ICommand<InterestDTos>
{
    public string? Name { get; set; }

    public Guid? InterestCategoryId { get; set; }
}