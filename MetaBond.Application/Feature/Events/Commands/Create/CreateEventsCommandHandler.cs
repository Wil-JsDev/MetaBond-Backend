using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Events.Commands.Create
{
    internal sealed class CreateEventsCommandHandler(
        IEventsRepository eventsRepository,
        ILogger<CreateEventsCommandHandler> logger)
        : ICommandHandler<CreateEventsCommand, EventsDto>
    {
        public async Task<ResultT<EventsDto>> Handle(
            CreateEventsCommand request,
            CancellationToken cancellationToken)
        {
            Domain.Models.Events events = new()
            {
                Id = Guid.NewGuid(),
                Description = request.Description,
                Title = request.Title,
                DateAndTime = request.DateAndTime,
                CommunitiesId = request.CommunitiesId
            };

            await eventsRepository.CreateAsync(events, cancellationToken);

            logger.LogInformation("Community {CommunityId} created successfully.", events.Id);

            var eventsDto = EventsMapper.EventsToDto(events);

            return ResultT<EventsDto>.Success(eventsDto);
        }
    }
}