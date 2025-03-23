using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Posts;

namespace MetaBond.Application.Feature.Posts.Query.GetById
{
    public sealed class GetByIdPostsQuerys : IQuery<PostsDTos>
    {
        public Guid PostsId { get; set; }
    }
}
