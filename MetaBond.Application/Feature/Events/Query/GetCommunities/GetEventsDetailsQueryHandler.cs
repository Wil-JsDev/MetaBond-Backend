using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communties;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using MetaBond.Domain.Models;
using Microsoft.Extensions.Logging;
using Serilog.Core;

namespace MetaBond.Application.Feature.Events.Query.GetCommunitiesAndParticipationInEvent
{
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
            if (!eventsDetails.Any())
            {
                logger.LogError("No communities or participation in event found for event with ID {Id}.", request.Id);

                return ResultT<IEnumerable<DTOs.Events.CommunitiesDTos>>.Failure(Error.NotFound("404", "No communities or participation in event found for this events."));
            }

            IEnumerable<DTOs.Events.CommunitiesDTos> inEventDTos = eventsDetails.Select(e => new DTOs.Events.CommunitiesDTos
            (
                    Id: e.Id,
                    Description: e.Description,
                    Title: e.Title,
                    DateAndTime: e.DateAndTime,
                    CreatedAt: e.CreateAt,
                    Communities: e.Communities ?? new Domain.Models.Communities()
            ));

            logger.LogInformation("Event details with ID {Id} retrieved successfully.", request.Id);

            return ResultT<IEnumerable<DTOs.Events.CommunitiesDTos>>.Success(inEventDTos);
        }
    }
}
