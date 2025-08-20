using MetaBond.Application.DTOs.Communities;
using MetaBond.Application.DTOs.Events;
using MetaBond.Domain.Models;
using CommunitiesDTos = MetaBond.Application.DTOs.Communities.CommunitiesDTos;

namespace MetaBond.Application.Mapper;

public static class CommunityMapper
{
    public static CommunitiesDTos MapCommunitiesDTos(Communities communities)
    {
        return new CommunitiesDTos
        (
            CommunitiesId: communities.Id,
            Name: communities.Name,
            Category: communities.Category,
            CreatedAt: communities.CreateAt
        );
    }

    public static PostsAndEventsDTos ToDtos(Communities community, IEnumerable<Posts>? posts,
        IEnumerable<Events>? events)
    {
        var postDtos = posts?.Select(PostsMapper.ToDTos);
        var eventDtos = events?.Select(EventsMapper.ToDTo);

        return new PostsAndEventsDTos(
            CommunitiesId: community.Id,
            Name: community.Name,
            Category: community.Category,
            CreatedAt: community.CreateAt,
            Posts: postDtos,
            Events: eventDtos
        );
    }

    public static IEnumerable<CommunitiesEventsDTos> ToCommunitiesDtos(this IEnumerable<Events> events)
    {
        return events.Select(e => new CommunitiesEventsDTos(
            Id: e.Id,
            Description: e.Description,
            Title: e.Title,
            DateAndTime: e.DateAndTime,
            CreatedAt: e.CreateAt,
            Communities: e.Communities != null
                ? new List<CommunitySummaryDto>
                {
                    new CommunitySummaryDto(
                        Description: e.Communities.Description,
                        Category: e.Communities.Category,
                        CreatedAt: e.Communities.CreateAt
                    )
                }
                : new List<CommunitySummaryDto>()
        ));
    }
}