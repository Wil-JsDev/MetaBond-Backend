using MetaBond.Domain.Models;

namespace MetaBond.Application.Mapper;

public static class EventsMapper
{
    public static Events ToDTo(Events events)
    {
        return new ()
        {
            Id = events.Id,
            Description = events.Description,
            Title = events.Title,
            DateAndTime = events.DateAndTime,
            CreateAt = events.CreateAt,
            CommunitiesId = events.CommunitiesId
        };
    }
}