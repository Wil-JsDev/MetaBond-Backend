using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;

namespace MetaBond.Application.Feature.Events.Query.FilterByDateRange
{
    internal sealed class FilterByDateRangeEventsQueryHandler : IQueryHandler<FilterByDateRangeEventsQuery, IEnumerable<EventsDto>>
    {
        private readonly IEventsRepository _eventsRepository;

        public FilterByDateRangeEventsQueryHandler(IEventsRepository eventsRepository)
        {
            _eventsRepository = eventsRepository;
        }

        public async Task<ResultT<IEnumerable<EventsDto>>> Handle(FilterByDateRangeEventsQuery request, CancellationToken cancellationToken)
        {

            DateTime filter = DateTime.UtcNow;
            if (request.DateRangeFilter == Domain.DateRangeFilter.LastDay)
            {
                filter = filter.AddDays(-1);
                var filterDay = await _eventsRepository.FilterByDateRange(filter,cancellationToken);
                IEnumerable<EventsDto> dTos = filterDay.Select(e => new EventsDto
                (
                    Id: e.Id,
                    Description: e.Description,
                    Title: e.Title,
                    DateAndTime: e.DateAndTime,
                    CreatedAt: e.CreateAt,
                    CommunitiesId: e.CommunitiesId,
                    ParticipationInEventId: e.ParticipationInEventId
                ));

                return ResultT<IEnumerable<EventsDto>>.Success(dTos);
            }
            else if (request.DateRangeFilter == Domain.DateRangeFilter.ThreeDays)
            {

                filter = filter.AddDays(-3);
                var filterDay = await _eventsRepository.FilterByDateRange(filter, cancellationToken);
                IEnumerable<EventsDto> dTos = filterDay.Select(e => new EventsDto
                (
                    Id: e.Id,
                    Description: e.Description,
                    Title: e.Title,
                    DateAndTime: e.DateAndTime,
                    CreatedAt: e.CreateAt,
                    CommunitiesId: e.CommunitiesId,
                    ParticipationInEventId: e.ParticipationInEventId
                ));

                return ResultT<IEnumerable<EventsDto>>.Success(dTos);
            }
            else if (request.DateRangeFilter == Domain.DateRangeFilter.LastWeek)
            {

                filter = filter.AddDays(-3);
                var filterDay = await _eventsRepository.FilterByDateRange(filter, cancellationToken);
                IEnumerable<EventsDto> dTos = filterDay.Select(e => new EventsDto
                (
                    Id: e.Id,
                    Description: e.Description,
                    Title: e.Title,
                    DateAndTime: e.DateAndTime,
                    CreatedAt: e.CreateAt,
                    CommunitiesId: e.CommunitiesId,
                    ParticipationInEventId: e.ParticipationInEventId
                ));

                return ResultT<IEnumerable<EventsDto>>.Success(dTos);
            }

            return ResultT<IEnumerable<EventsDto>>.Failure(Error.Failure("400", "No data entered"));
        }
    }
}
