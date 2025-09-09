using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Interest;

namespace MetaBond.Application.Feature.Interest.Query.GetById;

public sealed class GetByIdInterestQuery : IQuery<InterestDTos>
{
    public Guid? InterestId { get; set; }
}