using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Application.Feature.Events.Query.Pagination
{
    internal sealed class GetPagedEventsQueryHandler : IQueryHandler<GetPagedEventsQuery, PagedResult<EventsDto>>
    {
        private readonly IEventsRepository _eventsRepository;

        public GetPagedEventsQueryHandler(IEventsRepository eventsRepository)
        {
            _eventsRepository = eventsRepository;
        }

        public async Task<ResultT<PagedResult<EventsDto>>> Handle(GetPagedEventsQuery request, CancellationToken cancellationToken)
        {
            if (request != null)
            {
                var eventsPaged = await _eventsRepository.GetPagedEventsAsync(request.PageNumber, request.PageSize, cancellationToken);

                var dtoItems = eventsPaged.Items.Select(e => new EventsDto
                (
                    Id: e.Id,
                    Description: e.Description,
                    Title: e.Title,
                    DateAndTime: e.DateAndTime,
                    CreatedAt: e.CreateAt,
                    CommunitiesId: e.CommunitiesId,
                    ParticipationInEventId: e.ParticipationInEventId
                ));

                PagedResult<EventsDto> result = new()
                {
                    TotalItems = eventsPaged.TotalItems,
                    CurrentPage = eventsPaged.CurrentPage,
                    TotalPages = eventsPaged.TotalPages,
                    Items = dtoItems
                };
                return ResultT<PagedResult<EventsDto>>.Success(result);
            }
            return ResultT < PagedResult<EventsDto>>.Failure
                (Error.Failure("400", "No events were found for the specified criteria."));
        }
    }
}
