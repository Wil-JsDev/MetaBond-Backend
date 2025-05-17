using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ParticipationInEventDtos;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ParticipationInEvent.Query.Pagination;

internal sealed class GetPagedParticipationInEventQueryHandler(
    IParticipationInEventRepository repository,
    IDistributedCache decoratedCache,
    ILogger<GetPagedParticipationInEventQueryHandler> logger)
    : IQueryHandler<GetPagedParticipationInEventQuery, PagedResult<ParticipationInEventDTos>>
{
    public async Task<ResultT<PagedResult<ParticipationInEventDTos>>> Handle(
        GetPagedParticipationInEventQuery request, 
        CancellationToken cancellationToken)
    {
        if (request != null)
        {
            string cacheKey = $"get-participation-in-event-paged-{request.PageNumber}-size-{request.PageSize}";
            var participationInEvent = await decoratedCache.GetOrCreateAsync(
                cacheKey,
                async () => await repository.GetPagedParticipationInEventAsync(
                    request.PageNumber, 
                    request.PageSize,
                    cancellationToken), 
                cancellationToken: cancellationToken);

            var dtoItems = participationInEvent.Items!.Select(p => new ParticipationInEventDTos
            (
                ParticipationInEventId: p.Id,
                EventId: p.EventId
            )).ToList();

            if (!dtoItems.Any())
            {
                logger.LogWarning("No participation found for the given page: {PageNumber}, size: {PageSize}.", 
                    request.PageNumber, request.PageSize);

                return ResultT<PagedResult<ParticipationInEventDTos>>.Failure(Error.NotFound("400", "No participation found"));
            }

            PagedResult<ParticipationInEventDTos> result = new()
            {
                TotalItems = participationInEvent.TotalItems,
                CurrentPage = participationInEvent.CurrentPage,
                TotalPages = participationInEvent.TotalPages,
                Items = dtoItems
            };

            logger.LogInformation("Successfully retrieved {TotalItems} participation for page {PageNumber} of {TotalPages}.",
                participationInEvent.TotalItems, request.PageNumber, participationInEvent.TotalPages);


            return ResultT<PagedResult<ParticipationInEventDTos>>.Success(result);
        }
        logger.LogError("Invalid request: The provided query parameters are null.");

        return ResultT<PagedResult<ParticipationInEventDTos>>.Failure(Error.Failure("400", "Bad request: Invalid query parameters"));
    }
}