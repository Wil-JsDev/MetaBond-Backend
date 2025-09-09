using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.DTOs.Posts;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Mapper;

public static class PostsMapper
{
    public static Domain.Models.Posts ToDTos(Domain.Models.Posts posts)
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

    public static IEnumerable<PostsWithUserDTos> ToPostsWithUserDtos(
        this IEnumerable<Posts> posts)
    {
        return posts.Select(x => new PostsWithUserDTos(
            PostsId: x.Id,
            Title: x.Title,
            Content: x.Content,
            ImageUrl: x.Image,
            CreatedBy: x.CreatedBy != null
                ? new UserPostsDTos(
                    UserId: x.CreatedBy.Id,
                    Username: x.CreatedBy.Username,
                    FirstName: x.CreatedBy.FirstName,
                    LastName: x.CreatedBy.LastName,
                    Photo: x.CreatedBy.Photo
                )
                : null,
            CommunitiesId: x.CommunitiesId
        ));
    }

    public static IEnumerable<PostsWithCommunitiesDTos> ToPostsWithCommunitiesDtos(
        this IEnumerable<Posts> posts)
    {
        return posts.Select(x => new PostsWithCommunitiesDTos(
            PostsId: x.Id,
            Title: x.Title,
            Content: x.Content,
            ImageUrl: x.Image,
            Communities: x.Communities != null
                ? new List<CommunitySummaryDto>
                {
                    new CommunitySummaryDto(
                        Description: x.Communities.Description,
                        CreatedAt: x.Communities.CreateAt
                    )
                }
                : new List<CommunitySummaryDto>(),
            CreatedAt: x.CreatedAt
        ));
    }

    public static PostsDTos PostsToDto(Posts posts)
    {
        return new PostsDTos
        (
            PostsId: posts.Id,
            Title: posts.Title,
            Content: posts.Content,
            ImageUrl: posts.Image,
            CreatedById: posts.CreatedById,
            CommunitiesId: posts.CommunitiesId,
            CreatedAt: posts.CreatedAt
        );
    }
}