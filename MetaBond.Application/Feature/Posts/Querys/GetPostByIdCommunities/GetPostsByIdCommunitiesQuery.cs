﻿
using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Posts;

namespace MetaBond.Application.Feature.Posts.Querys.GetPostByIdCommunities
{
    public sealed class GetPostsByIdCommunitiesQuery : IQuery<IEnumerable<PostsWithCommunitiesDTos>>
    {
        public Guid PostsId { get; set; }
    }
}
