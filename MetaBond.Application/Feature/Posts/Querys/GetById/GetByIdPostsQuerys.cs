using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Posts;

namespace MetaBond.Application.Feature.Posts.Querys.GetById
{
    public sealed class GetByIdPostsQuerys : IQuery<PostsDTos>
    {
        public Guid PostsId { get; set; }
    }
}
