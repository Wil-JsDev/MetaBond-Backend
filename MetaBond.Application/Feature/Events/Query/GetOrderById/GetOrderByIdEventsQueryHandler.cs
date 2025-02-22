using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Events.Query.GetOrderById
{
    internal sealed class GetOrderByIdEventsQueryHandler : IQueryHandler<GetOrderByIdEventsQuery, IEnumerable<EventsDto>>
    {
        private readonly IEventsRepository _eventsRepository;
        private readonly ILogger<GetOrderByIdEventsQueryHandler> _logger;

        public GetOrderByIdEventsQueryHandler(
            IEventsRepository eventsRepository, 
            ILogger<GetOrderByIdEventsQueryHandler> logger)
        {
            _eventsRepository = eventsRepository;
            _logger = logger;
        }

        public async Task<ResultT<IEnumerable<EventsDto>>> Handle(GetOrderByIdEventsQuery request, CancellationToken cancellationToken)
        {
            var eventsSort = GetSortValues();
            if (eventsSort.TryGetValue((request.Order.ToUpper()), out var GetSort))
            {
                var eventsListSort = await GetSort(cancellationToken);
                if (eventsListSort == null || !eventsListSort.Any())
                {
                    _logger.LogError("No events found for the specified order.");

                    return ResultT<IEnumerable<EventsDto>>.Failure(Error.Failure("400", "No events found for the given order."));
                }

                IEnumerable<EventsDto> eventsDtos = eventsListSort.Select(x => new EventsDto
                (
                    Id: x.Id,
                    Description: x.Description,
                    Title: x.Title,
                    DateAndTime: x.DateAndTime,
                    CreatedAt: x.CreateAt,
                    CommunitiesId: x.CommunitiesId
                ));

                _logger.LogInformation("Successfully retrieved {Count} events ordered by {Order}.", eventsDtos.Count(), request.Order);

                return ResultT<IEnumerable<EventsDto>>.Success(eventsDtos);
            }
            _logger.LogError("Invalid order parameter received: {Order}. Please specify 'asc' or 'desc'.", request.Order);

            return ResultT<IEnumerable<EventsDto>>.Failure(Error.Failure("400", "Invalid order parameter. Please specify 'asc' or 'desc'."));
        }

        private Dictionary<string, Func<CancellationToken, Task<IEnumerable<Domain.Models.Events>>>> GetSortValues()
        {
            return new Dictionary<string, Func<CancellationToken, Task<IEnumerable<Domain.Models.Events>>>>
            {
                {"asc", async cancellationToken => await _eventsRepository.GetOrderByIdAscAsync(cancellationToken) },
                {"desc", async cancellationToken => await _eventsRepository.GetOrderByIdDescAsync(cancellationToken) }
            };
        }

    }
}
