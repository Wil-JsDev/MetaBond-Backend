using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.DTOs.ParticipationInEventDtos;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using MetaBond.Domain.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ParticipationInEvent.Query.GetEvents;

internal sealed class GetEventsQueryHandler(
    IParticipationInEventRepository participationInEventRepository,
    IDistributedCache decoratedCache,
    ILogger<GetEventsQueryHandler> logger)
    : IQueryHandler<GetEventsQuery, PagedResult<EventsWithParticipationInEventDTos>>
{
    public async Task<ResultT<PagedResult<EventsWithParticipationInEventDTos>>> Handle(
        GetEventsQuery request,
        CancellationToken cancellationToken)
    {
        var participationInEventId = await EntityHelper.GetEntityByIdAsync(
            participationInEventRepository.GetByIdAsync,
            request.ParticipationInEventId ?? Guid.Empty,
            "ParticipationInEvent",
            logger
        );

        if (!participationInEventId.IsSuccess) return participationInEventId.Error!;

        var paginationValidation = PaginationHelper.ValidatePagination<EventsWithParticipationInEventDTos>(
            request.PageNumber,
            request.PageSize, logger);

        if (!paginationValidation.IsSuccess)
            return paginationValidation.Error!;

        string cacheKey =
            $"GetEventsQueryHandler-{request.ParticipationInEventId}-size-{request.PageSize}--page-{request.PageNumber}";

        var result = await decoratedCache.GetOrCreateAsync(
            cacheKey,
            async () =>
            {
                var participationInEvents = await participationInEventRepository.GetEventsAsync(
                    participationInEventId.Value.Id,
                    request.PageNumber,
                    request.PageSize,
                    cancellationToken);

                if (participationInEvents.Items == null) return null;
                var pagedDto = participationInEvents.Items.ToParticipationInEventDtos();

                PagedResult<EventsWithParticipationInEventDTos> result = new(
                    totalItems: participationInEvents.TotalItems,
                    currentPage: participationInEvents.CurrentPage,
                    pageSize: participationInEvents.TotalPages,
                    items: pagedDto
                );
                return result;
            },
            cancellationToken: cancellationToken);


        if (!result.Items.Any())
        {
            logger.LogError("No events found for participation in event with ID: {ParticipationInEventId}",
                participationInEventId.Value.Id);

            return ResultT<PagedResult<EventsWithParticipationInEventDTos>>.Failure(Error.Failure("400",
                "The list is empty"));
        }

        logger.LogInformation(
            "Successfully retrieved {Count} events for participation in event with ID: {ParticipationInEventId}",
            result.Items.Count(), participationInEventId.Value.Id);

        return ResultT<PagedResult<EventsWithParticipationInEventDTos>>.Success(result);
    }
}