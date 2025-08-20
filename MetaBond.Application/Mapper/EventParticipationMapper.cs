using MetaBond.Application.DTOs.ParticipationInEventDtos;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Mapper;

public static class EventParticipationMapper
{
    public static ParticipationInEventDTos EventParticipationToDto(EventParticipation inEvent)
    {
        return new ParticipationInEventDTos
        (
            ParticipationInEventId: inEvent.ParticipationInEventId,
            EventId: inEvent.EventId
        );
    }
}