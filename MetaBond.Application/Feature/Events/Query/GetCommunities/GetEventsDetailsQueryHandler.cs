using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Feature.Events.Query.GetCommunitiesAndParticipationInEvent;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Events.Query.GetCommunities;

    internal sealed class GetEventsDetailsQueryHandler(
        IEventsRepository eventsRepository,
        ILogger<GetEventsDetailsQueryHandler> logger)
        : IQueryHandler<GetEventsDetailsQuery, IEnumerable<DTOs.Events.CommunitiesDTos>>
    {
        public async Task<ResultT<IEnumerable<DTOs.Events.CommunitiesDTos>>> Handle(
            GetEventsDetailsQuery request, 
            CancellationToken cancellationToken)
        {
            var events = await eventsRepository.GetByIdAsync(request.Id);
            if (events == null)
            {
                logger.LogError("Event with ID {Id} not found.", request.Id);

                return ResultT<IEnumerable<DTOs.Events.CommunitiesDTos>>.Failure(Error.NotFound("404", $"{request.Id} not found"));
            }

            var eventsDetails = await eventsRepository.GetCommunities(request.Id,cancellationToken);
            var eventsEnumerable = eventsDetails.ToList();
            if (!eventsEnumerable.Any())
            {
                logger.LogError("No communities or participation in event found for event with ID {Id}.", request.Id);

                return ResultT<IEnumerable<DTOs.Events.CommunitiesDTos>>.Failure(Error.NotFound("404", "No communities or participation in event found for this events."));
            }
            
            IEnumerable<DTOs.Events.CommunitiesDTos> inEventDTos = eventsEnumerable.Select(e => new DTOs.Events.CommunitiesDTos
            (
                    Id: e.Id,
                    Description: e.Description,
                    Title: e.Title,
                    DateAndTime: e.DateAndTime,
                    CreatedAt: e.CreateAt,
                    Communities: e.Communities != null
                        ?
                        [
                            new CommunitySummaryDto(
                                e.Communities.Description,
                                e.Communities.Category,
                                e.Communities.CreateAt
                            )
                        ]
                        : new List<CommunitySummaryDto>()
            ));
            logger.LogInformation("Event details with ID {Id} retrieved successfully.", request.Id);

            return ResultT<IEnumerable<DTOs.Events.CommunitiesDTos>>.Success(inEventDTos);
        }
    }
