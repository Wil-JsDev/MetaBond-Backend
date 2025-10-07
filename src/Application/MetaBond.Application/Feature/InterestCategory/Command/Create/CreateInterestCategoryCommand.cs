using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.InterestCategory;

namespace MetaBond.Application.Feature.InterestCategory.Command.Create;

public sealed class CreateInterestCategoryCommand : ICommand<InterestCategoryDTos>
{
    public string? Name { get; set; }
}