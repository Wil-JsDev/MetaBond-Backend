using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressEntry.Querys.GetById
{
    internal sealed class GetByIdProgressEntryQueryHandler : IQueryHandler<GetByIdProgressEntryQuery, ProgressEntryDTos>
    {
        private readonly IProgressEntryRepository _progressEntryRepository;
        private readonly ILogger<GetByIdProgressEntryQueryHandler> _logger;

        public GetByIdProgressEntryQueryHandler(IProgressEntryRepository progressEntryRepository, ILogger<GetByIdProgressEntryQueryHandler> logger)
        {
            _progressEntryRepository = progressEntryRepository;
            _logger = logger;
        }

        public async Task<ResultT<ProgressEntryDTos>> Handle(GetByIdProgressEntryQuery request, CancellationToken cancellationToken)
        {
            var progressEntry = await _progressEntryRepository.GetByIdAsync(request.ProgressEntryId);
            if (progressEntry != null)
            {
                ProgressEntryDTos entryDTos = new
                (
                    ProgressEntryId: progressEntry.Id,
                    ProgressBoardId: progressEntry.ProgressBoardId,
                    Description: progressEntry.Description,
                    CreatedAt: progressEntry.CreatedAt,
                    UpdateAt: progressEntry.UpdateAt
                );

                _logger.LogInformation("Progress entry with ID {Id} retrieved successfully.", progressEntry.Id);

                return ResultT<ProgressEntryDTos>.Success(entryDTos);
            }

            _logger.LogError("Progress entry with ID {Id} not found.", request.ProgressEntryId);

            return ResultT<ProgressEntryDTos>.Failure(Error.NotFound("400",$"{request.ProgressEntryId} not found"));
        }
    }
}
