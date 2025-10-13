using MetaBond.Application.DTOs.Events;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Mapper;

public static class EventsMapper
{
    public static Events ToDTo(Events events)
    {
        return new()
        {
            Id = events.Id,
            Description = events.Description,
            Title = events.Title,
            DateAndTime = events.DateAndTime,
            CreateAt = events.CreateAt,
            CommunitiesId = events.CommunitiesId
        };
    }

    public static EventsDto EventsToDto(Events events)
    {
        return new EventsDto
        (
            Id: events.Id,
            Description: events.Description,
            Title: events.Title,
            DateAndTime: events.DateAndTime,
            CreatedAt: events.CreateAt,
            CommunitiesId: events.CommunitiesId
        );
    }

    public static IEnumerable<EventsWithParticipationInEventsDTos> ToEventsWithParticipationDtos(
        this IEnumerable<Events> events)
    {
        return events.Select(x => new EventsWithParticipationInEventsDTos(
            EventsId: x.Id,
            Description: x.Description,
            Title: x.Title,
            DateAndTime: x.DateAndTime,
            ParticipationInEvents: x.EventParticipations != null
                ? x.EventParticipations.Select(ep => new ParticipationInEventBasicDTos(
                    ParticipationInEventId: ep.ParticipationInEventId,
                    EventId: ep.EventId
                ))
                : [],
            CreatedAt: x.CreateAt
        ));
    }
}