using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ParticipationInEvent;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ParticipationInEvent.Querys.Pagination
{
    internal sealed class GetPagedParticipationInEventQueryHandler : IQueryHandler<GetPagedParticipationInEventQuery, PagedResult<ParticipationInEventDTos>>
    {
        private readonly IParticipationInEventRepository _repository;
        private readonly ILogger<GetPagedParticipationInEventQueryHandler> _logger;

        public GetPagedParticipationInEventQueryHandler(
            IParticipationInEventRepository repository, 
            ILogger<GetPagedParticipationInEventQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ResultT<PagedResult<ParticipationInEventDTos>>> Handle(
            GetPagedParticipationInEventQuery request, 
            CancellationToken cancellationToken)
        {
            if (request != null)
            {
                var participationInEvent = await _repository.GetPagedParticipationInEventAsync(
                    request.PageNumber,
                    request.PageSize,
                    cancellationToken);

                var dtoItems = participationInEvent.Items.Select(p => new ParticipationInEventDTos
                (
                    ParticipationInEventId: p.Id,
                    EventId: p.EventId
                )).ToList();

                if (!dtoItems.Any())
                {
                    _logger.LogWarning("No participation found for the given page: {PageNumber}, size: {PageSize}.", 
                                        request.PageNumber, request.PageSize);

                    return ResultT<PagedResult<ParticipationInEventDTos>>.Failure(Error.NotFound("400", "No participations found"));
                }

                PagedResult<ParticipationInEventDTos> result = new()
                {
                    TotalItems = participationInEvent.TotalItems,
                    CurrentPage = participationInEvent.CurrentPage,
                    TotalPages = participationInEvent.TotalPages,
                    Items = dtoItems
                };

                _logger.LogInformation("Successfully retrieved {TotalItems} participations for page {PageNumber} of {TotalPages}.",
                                        participationInEvent.TotalItems, request.PageNumber, participationInEvent.TotalPages);


                return ResultT<PagedResult<ParticipationInEventDTos>>.Success(result);
            }
            _logger.LogError("Invalid request: The provided query parameters are null.");

            return ResultT<PagedResult<ParticipationInEventDTos>>.Failure(Error.Failure("400", "Bad request: Invalid query parameters"));
        }
    }
}
