using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Events.Query.Pagination
{
    internal sealed class GetPagedEventsQueryHandler(
        IEventsRepository eventsRepository,
        IDistributedCache decoratedCache,
        ILogger<GetPagedEventsQueryHandler> logger)
        : IQueryHandler<GetPagedEventsQuery, PagedResult<EventsDto>>
    {
        public async Task<ResultT<PagedResult<EventsDto>>> Handle(GetPagedEventsQuery request, CancellationToken cancellationToken)
        {
            if (request != null)
            {

                if (request.PageNumber <= 0 && request.PageSize <= 0)
                {
                    logger.LogError("Invalid pagination parameters");

                    return ResultT<PagedResult<EventsDto>>.Failure(Error.Failure("400", "PageNumber and PageSize must be greater than 0."));
                }
                
                string cacheKey = $"paged-events-page-{request.PageNumber}-size-{request.PageSize}";

                var eventsPaged = await decoratedCache.GetOrCreateAsync(
                    cacheKey,
                    async () => await eventsRepository.GetPagedEventsAsync(request.PageNumber, request.PageSize,
                        cancellationToken), 
                    cancellationToken: cancellationToken);

                var dtoItems = eventsPaged.Items!.Select(e => new EventsDto
                (
                    Id: e.Id,
                    Description: e.Description,
                    Title: e.Title,
                    DateAndTime: e.DateAndTime,
                    CreatedAt: e.CreateAt,
                    CommunitiesId: e.CommunitiesId
                ));

                PagedResult<EventsDto> result = new()
                {
                    TotalItems = eventsPaged.TotalItems,
                    CurrentPage = eventsPaged.CurrentPage,
                    TotalPages = eventsPaged.TotalPages,
                    Items = dtoItems
                };

                logger.LogInformation("Paged events retrieved successfully. Page {PageNumber} of {TotalPages}.", 
                    request.PageNumber, eventsPaged.TotalPages);

                return ResultT<PagedResult<EventsDto>>.Success(result);
            }

            logger.LogError("No events were found for the specified criteria. Page {PageNumber}.", request!.PageNumber);

            return ResultT<PagedResult<EventsDto>>.Failure
                (Error.Failure("400", "No events were found for the specified criteria."));

        }
    }
}
