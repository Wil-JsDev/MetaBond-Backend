using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Events.Query.GetOrderById
{
    internal sealed class GetOrderByIdEventsQueryHandler(
        IEventsRepository eventsRepository,
        IDistributedCache decoratedCache,
        ILogger<GetOrderByIdEventsQueryHandler> logger)
        : IQueryHandler<GetOrderByIdEventsQuery, IEnumerable<EventsDto>>
    {
        public async Task<ResultT<IEnumerable<EventsDto>>> Handle(GetOrderByIdEventsQuery request,
            CancellationToken cancellationToken)
        {
            var eventsSort = GetSortValues();
            if (eventsSort.TryGetValue((request.Order!), out var getSort))
            {
                var eventsListSort = await decoratedCache.GetOrCreateAsync(
                    $"order-{request.Order}",
                    async () =>
                    {
                        var eventSort = await getSort(cancellationToken);
                        IEnumerable<EventsDto> eventsDtos = eventSort.Select(EventsMapper.EventsToDto).ToList();

                        return eventsDtos;
                    },
                    cancellationToken: cancellationToken);

                var listSort = eventsListSort.ToList();
                if (!listSort.Any())
                {
                    logger.LogError("No events found for the specified order.");

                    return ResultT<IEnumerable<EventsDto>>.Failure(Error.Failure("400",
                        "No events found for the given order."));
                }

                IEnumerable<EventsDto> value = listSort.ToList();
                logger.LogInformation("Successfully retrieved {Count} events ordered by {Order}.", value.Count(),
                    request.Order);

                return ResultT<IEnumerable<EventsDto>>.Success(value);
            }

            logger.LogError("Invalid order parameter received: {Order}. Please specify 'asc' or 'desc'.",
                request.Order);

            return ResultT<IEnumerable<EventsDto>>.Failure(Error.Failure("400",
                "Invalid order parameter. Please specify 'asc' or 'desc'."));
        }

        #region Private Methods

        private Dictionary<string, Func<CancellationToken, Task<IEnumerable<Domain.Models.Events>>>> GetSortValues()
        {
            return new Dictionary<string, Func<CancellationToken, Task<IEnumerable<Domain.Models.Events>>>>
            {
                { "asc", async cancellationToken => await eventsRepository.GetOrderByIdAscAsync(cancellationToken) },
                { "desc", async cancellationToken => await eventsRepository.GetOrderByIdDescAsync(cancellationToken) }
            };
        }

        #endregion
    }
}