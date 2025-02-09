using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressEntry.Querys.GetAll
{
    internal sealed class GetAllProgressEntryQueryHandler : IQueryHandler<GetAllProgressEntryQuery, IEnumerable<ProgressEntryDTos>>
    {
        private readonly IProgressEntryRepository _progressEntryRepository;
        private readonly ILogger<GetAllProgressEntryQueryHandler> _logger;

        public GetAllProgressEntryQueryHandler(
            IProgressEntryRepository progressEntryRepository, 
            ILogger<GetAllProgressEntryQueryHandler> logger)
        {
            _progressEntryRepository = progressEntryRepository;
            _logger = logger;
        }

        public async Task<ResultT<IEnumerable<ProgressEntryDTos>>> Handle(
            GetAllProgressEntryQuery request, 
            CancellationToken cancellationToken)
        {

            IEnumerable<Domain.Models.ProgressEntry> progressEntries = await _progressEntryRepository.GetAll(cancellationToken);
            if ( progressEntries == null || !progressEntries.Any())
            {
                _logger.LogError("No progress entries found");

                return ResultT<IEnumerable<ProgressEntryDTos>>.Failure(Error.Failure("400","The list is empty"));
            }

            IEnumerable<ProgressEntryDTos> dTos = progressEntries.Select(x => new ProgressEntryDTos
            (
                ProgressEntryId: x.Id,
                ProgressBoardId: x.ProgressBoardId,
                Description: x.Description,
                CreatedAt: x.CreatedAt,
                UpdateAt: x.UpdateAt
            ));


            _logger.LogInformation("Successfully retrieved {Count} progress entries.", dTos.Count());

            return ResultT<IEnumerable<ProgressEntryDTos>>.Success(dTos);
        }
    }
}
