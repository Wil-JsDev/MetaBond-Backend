using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.InterestCategory;

namespace MetaBond.Application.Feature.InterestCategory.Query.GetById;

public class GetByIdInterestCategoryQuery : IQuery<InterestCategoryGeneralDTos>
{
    public Guid InterestCategoryId { get; set; }
}