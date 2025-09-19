using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communities;

namespace MetaBond.Application.Feature.Communities.Query.GetById;

public sealed class GetByIdCommunitiesQuery : IQuery<CommunitiesDTos>
{
    public Guid Id { get; set; }
}