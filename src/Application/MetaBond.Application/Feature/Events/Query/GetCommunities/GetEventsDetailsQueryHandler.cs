using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Events.Query.GetCommunities;

internal sealed class GetEventsDetailsQueryHandler(
    IEventsRepository eventsRepository,
    IDistributedCache decoratedCache,
    ILogger<GetEventsDetailsQueryHandler> logger)
    : IQueryHandler<GetEventsDetailsQuery, PagedResult<CommunitiesEventsDTos>>
{
    public async Task<ResultT<PagedResult<CommunitiesEventsDTos>>> Handle(
        GetEventsDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var events = await EntityHelper.GetEntityByIdAsync
        (
            eventsRepository.GetByIdAsync,
            request.Id,
            "Events",
            logger
        );
        if (!events.IsSuccess) return events.Error!;

        var result = await decoratedCache.GetOrCreateAsync(
            $"events-{request.Id}",
            async () =>
            {
                var communitiesEvents = await eventsRepository.GetCommunitiesAsync(request.Id,
                    request.PageNumber,
                    request.PageSize,
                    cancellationToken);

                IEnumerable<CommunitiesEventsDTos> inEventDTos = communitiesEvents.Items.ToCommunitiesDtos();

                PagedResult<CommunitiesEventsDTos> eventsPaged = new(
                    currentPage: communitiesEvents.CurrentPage,
                    items: inEventDTos,
                    totalItems: communitiesEvents.TotalItems,
                    pageSize: request.PageSize
                );

                return eventsPaged;
            },
            cancellationToken: cancellationToken
        );

        if (result.Items != null && !result.Items.Any())
        {
            logger.LogError("No communities or participation in event found for event with ID {Id}.", request.Id);

            return ResultT<PagedResult<CommunitiesEventsDTos>>.Failure(Error.NotFound("404",
                "No communities or participation in event found for this events."));
        }

        logger.LogInformation("Event details with ID {Id} retrieved successfully.", request.Id);

        return ResultT<PagedResult<CommunitiesEventsDTos>>.Success(result);
    }
}