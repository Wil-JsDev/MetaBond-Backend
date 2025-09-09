using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.InterestCategory;

namespace MetaBond.Application.Feature.InterestCategory.Command.Update;

public sealed class UpdateInterestCategoryCommand : ICommand<UpdateInterestCategoryDTos>
{
    public Guid InterestCategoryId { get; set; }
    
    public string? Name { get; set; }
}