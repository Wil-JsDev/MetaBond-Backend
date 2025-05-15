using MetaBond.Domain.Models;

namespace MetaBond.Application.Mapper;

public static class PostsMapper
{
    public static Domain.Models.Posts ToDTos(Domain.Models.Posts  posts)
    {

        return new Posts
        {
            Id = posts.Id,
            Title = posts.Title,
            Content = posts.Content,
            Image = posts.Image,
            CommunitiesId = posts.CommunitiesId,
            CreatedAt = posts.CreatedAt
        };

    }
}