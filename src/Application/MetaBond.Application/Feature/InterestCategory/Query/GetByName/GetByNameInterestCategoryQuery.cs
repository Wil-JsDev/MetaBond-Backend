using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.InterestCategory;

namespace MetaBond.Application.Feature.InterestCategory.Query.GetByName;

public sealed class GetByNameInterestCategoryQuery : IQuery<InterestCategoryGeneralDTos>
{
    public string? Name { get; set; }
}