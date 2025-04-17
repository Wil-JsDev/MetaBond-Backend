using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Events.Query.GetById
{
    internal sealed class GetByIdEventsQueryHandler(
        IEventsRepository eventsRepository,
        IDistributedCache decoratedCache,
        ILogger<GetByIdEventsQueryHandler> logger)
        : IQueryHandler<GetByIdEventsQuery, EventsDto>
    {
        public async Task<ResultT<EventsDto>> Handle(
            GetByIdEventsQuery request, 
            CancellationToken cancellationToken)
        {
            
            var events =  await decoratedCache.GetOrCreateAsync(
                $"events-{request.Id}",
                async () => await eventsRepository.GetByIdAsync(request.Id), 
                cancellationToken: cancellationToken);
            if (events != null)
            {
                EventsDto eventsDto = new
                (
                    Id: events.Id,
                    Description: events.Description,
                    Title: events.Title,
                    DateAndTime: events.DateAndTime,
                    CreatedAt: events.CreateAt,
                    CommunitiesId: events.CommunitiesId
                );

                logger.LogInformation("Event with ID {Id} retrieved successfully.", events.Id);

                return ResultT<EventsDto>.Success(eventsDto);

            }

            logger.LogError("Event with ID {Id} not found.", request.Id);

            return ResultT<EventsDto>.Failure(Error.NotFound("404", $"{request.Id} not found"));

        }
    }
}
