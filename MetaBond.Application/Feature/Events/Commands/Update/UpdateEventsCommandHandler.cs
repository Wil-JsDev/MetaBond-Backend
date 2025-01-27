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
                Domain.Models.Events eventsModel = new()
                {
                    Description = events.Description,
                    Title = events.Title,
                    ParticipationInEventId = events.ParticipationInEventId
                };

                await _eventsRepository.UpdateAsync(eventsModel,cancellationToken);

                EventsDto eventsDto = new
                (
                    Id: eventsModel.Id,
                    Description: eventsModel.Description,
                    Title: eventsModel.Title,
                    DateAndTime: eventsModel.DateAndTime,
                    CreatedAt: eventsModel.CreateAt,
                    CommunitiesId: eventsModel.CommunitiesId,
                    ParticipationInEventId: eventsModel.ParticipationInEventId
                );

                return ResultT<EventsDto>.Success(eventsDto);
            }

            return ResultT<EventsDto>.Failure(Error.NotFound("404", $"{request.Id} not found"));
        }
    }
}
