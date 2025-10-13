using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Posts;

namespace MetaBond.Application.Feature.Posts.Query.GetFilterTop10;

public sealed class GetFilterTop10Query : IQuery<IEnumerable<PostsDTos>>
{
    public Guid CommunitiesId { get; set; }
}