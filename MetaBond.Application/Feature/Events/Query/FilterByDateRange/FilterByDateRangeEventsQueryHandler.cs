using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Events.Query.FilterByDateRange
{
    internal sealed class FilterByDateRangeEventsQueryHandler : IQueryHandler<FilterByDateRangeEventsQuery, IEnumerable<EventsDto>>
    {
        private readonly IEventsRepository _eventsRepository;
        private readonly ILogger<FilterByDateRangeEventsQueryHandler> _logger;  

        public FilterByDateRangeEventsQueryHandler(
            IEventsRepository eventsRepository, 
            ILogger<FilterByDateRangeEventsQueryHandler> logger)
        {
            _eventsRepository = eventsRepository;
            _logger = logger;
        }

        public async Task<ResultT<IEnumerable<EventsDto>>> Handle(
            FilterByDateRangeEventsQuery request, 
            CancellationToken cancellationToken)
        {

            var status = GetStatus();
            if (status.TryGetValue((request.DateRangeFilter), out var getStatusList))
            {
                var eventsList = await getStatusList(cancellationToken);
                if (eventsList == null || !eventsList.Any())
                {
                    _logger.LogError("The events list is empty.");

                    return ResultT<IEnumerable<EventsDto>>.Failure(Error.Failure("400", "The list is empty"));

                }

                IEnumerable<EventsDto> dTos = eventsList.Select(e => new EventsDto
                (
                    Id: e.Id,
                    Description: e.Description,
                    Title: e.Title,
                    DateAndTime: e.DateAndTime,
                    CreatedAt: e.CreateAt,
                    CommunitiesId: e.CommunitiesId
                ));

                _logger.LogInformation("Events retrieved successfully");

                return ResultT<IEnumerable<EventsDto>>.Success(dTos);

            }

            _logger.LogError("No data entered for the status.");

            return ResultT<IEnumerable<EventsDto>>.Failure(Error.Failure("400", "No data entered"));

        }

        private Dictionary<Domain.DateRangeFilter, Func<CancellationToken,Task<IEnumerable<Domain.Models.Events>>>> GetStatus()
        {
            return new Dictionary<Domain.DateRangeFilter, Func<CancellationToken, Task<IEnumerable<Domain.Models.Events>>>>
            {
                { DateRangeFilter.LastDay, async cancellationToken => await _eventsRepository.FilterByDateRange(DateTime.UtcNow.AddDays(- 1),cancellationToken)  },
                { DateRangeFilter.ThreeDays, async cancellationToken => await _eventsRepository.FilterByDateRange(DateTime.UtcNow.AddDays(- 3),cancellationToken)  },
                { DateRangeFilter.LastWeek, async cancellationToken => await _eventsRepository.FilterByDateRange(DateTime.UtcNow.AddDays(- 7),cancellationToken)  }
            };
        }

    }
}
