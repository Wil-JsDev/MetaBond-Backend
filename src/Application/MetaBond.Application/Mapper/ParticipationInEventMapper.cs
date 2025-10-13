using MetaBond.Application.DTOs.Events;
using MetaBond.Application.DTOs.ParticipationInEventDtos;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Mapper;

public static class ParticipationInEventMapper
{
    public static IEnumerable<EventsWithParticipationInEventDTos> ToParticipationInEventDtos(
        this IEnumerable<ParticipationInEvent> participations)
    {
        return participations.Select(x => new EventsWithParticipationInEventDTos(
            ParticipationInEventId: x.Id,
            Events: x.EventParticipations != null
                ? x.EventParticipations.Select(ep => new EventsDto(
                    Id: ep.Event.Id,
                    Description: ep.Event.Description,
                    Title: ep.Event.Title,
                    DateAndTime: ep.Event.DateAndTime,
                    CreatedAt: ep.Event.CreateAt,
                    CommunitiesId: ep.Event.CommunitiesId
                ))
                : Enumerable.Empty<EventsDto>()
        ));
    }

    public static ParticipationInEventDTos ParticipationInEventToDto(ParticipationInEvent inEvent)
    {
        return new ParticipationInEventDTos
        (
            ParticipationInEventId: inEvent.Id,
            EventId: inEvent.EventId
        );
    }
}