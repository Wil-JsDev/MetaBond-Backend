using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ParticipationInEventDtos;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
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
        
        if (request is null)
        {
            logger.LogError("The request is null.");
            
            return ResultT<PagedResult<ParticipationInEventDTos>>.Failure(Error.Failure("400",
                "The request is null"));
        }
        
        string cacheKey = $"get-participation-in-event-paged-{request.PageNumber}-size-{request.PageSize}";
        var result = await decoratedCache.GetOrCreateAsync(
            cacheKey,
            async () =>
            {
                var participationInEvent = await repository.GetPagedParticipationInEventAsync(
                    request.PageNumber,
                    request.PageSize,
                    cancellationToken);


                var dtoItems = participationInEvent.Items!.Select(ParticipationInEventMapper.ParticipationInEventToDto).ToList();

                PagedResult<ParticipationInEventDTos> result = new()
                {
                    TotalItems = participationInEvent.TotalItems,
                    CurrentPage = participationInEvent.CurrentPage,
                    TotalPages = participationInEvent.TotalPages,
                    Items = dtoItems
                };

                return result;
            },
            cancellationToken: cancellationToken);


        if (!result.Items!.Any())
        {
            logger.LogWarning("No participation found for the given page: {PageNumber}, size: {PageSize}.",
                request.PageNumber, request.PageSize);

            return ResultT<PagedResult<ParticipationInEventDTos>>.Failure(Error.NotFound("400",
                "No participation found"));
        }

        logger.LogInformation(
            "Successfully retrieved {TotalItems} participation for page {PageNumber} of {TotalPages}.",
            result.TotalItems, request.PageNumber, result.TotalPages);
        
        return ResultT<PagedResult<ParticipationInEventDTos>>.Success(result);
    }
}