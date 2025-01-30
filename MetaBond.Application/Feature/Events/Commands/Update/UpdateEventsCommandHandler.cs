using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;

namespace MetaBond.Application.Feature.Events.Commands.Update
{
    internal sealed class UpdateEventsCommandHandler : ICommandHandler<UpdateEventsCommand, EventsDto>
    {
        private readonly IEventsRepository _eventsRepository;

        public UpdateEventsCommandHandler(IEventsRepository eventsRepository)
        {
            _eventsRepository = eventsRepository;
        }

        public async Task<ResultT<EventsDto>> Handle(UpdateEventsCommand request, CancellationToken cancellationToken)
        {
            var events = await _eventsRepository.GetByIdAsync(request.Id);
            if (events != null)
            {
                events.Description = request.Description;
                events.Title = request.Title;
                events.ParticipationInEventId = request.ParticipationInEventId;

                await _eventsRepository.UpdateAsync(events, cancellationToken);

                EventsDto eventsDto = new
                (
                    Id: events.Id,
                    Description: events.Description,
                    Title: events.Title,
                    DateAndTime: events.DateAndTime,
                    CreatedAt: events.CreateAt,
                    CommunitiesId: events.CommunitiesId,
                    ParticipationInEventId: events.ParticipationInEventId
                );

                return ResultT<EventsDto>.Success(eventsDto);
            }

            return ResultT<EventsDto>.Failure(Error.NotFound("404", $"{request.Id} not found"));
        }
    }
}
