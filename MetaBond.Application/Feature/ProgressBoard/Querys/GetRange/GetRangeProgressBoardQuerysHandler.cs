using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressBoard.Querys.GetRange
{
    internal sealed class GetRangeProgressBoardQuerysHandler : IQueryHandler<GetRangeProgressBoardQuerys, IEnumerable<ProgressBoardWithProgressEntryDTos>>
    {

        private readonly IProgressBoardRepository _progressBoardRepository;
        private readonly ILogger<GetRangeProgressBoardQuerysHandler> _logger;

        public GetRangeProgressBoardQuerysHandler(
            IProgressBoardRepository progressBoardRepository, 
            ILogger<GetRangeProgressBoardQuerysHandler> logger)
        {
            _progressBoardRepository = progressBoardRepository;
            _logger = logger;
        }

        public async Task<ResultT<IEnumerable<ProgressBoardWithProgressEntryDTos>>> Handle(
            GetRangeProgressBoardQuerys request, 
            CancellationToken cancellationToken)
        {
            var progressBoard = GetValue();
            if (progressBoard.TryGetValue((request.DateRangeType), out var progressBoardValue))
            {
                var progressBoardList = await progressBoardValue(cancellationToken);
                if ( progressBoardList == null || !progressBoardList.Any())
                {
                    _logger.LogError("No progress board entries found for DateRangeType: {DateRangeType}", request.DateRangeType);

                    return ResultT<IEnumerable<ProgressBoardWithProgressEntryDTos>>.Failure(
                        Error.Failure("400", "The list is empty")
                    );
                }

                IEnumerable<ProgressBoardWithProgressEntryDTos> progressBoardWithProgressEntryDTos = progressBoardList.Select(x => new ProgressBoardWithProgressEntryDTos
                (
                    ProgressBoardId: x.Id,
                    CommunitiesId: x.CommunitiesId,
                    ProgressEntries: x.ProgressEntries,
                    CreatedAt: x.CreatedAt,
                    UpdatedAt: x.UpdatedAt
                ));

                _logger.LogInformation("Successfully retrieved {Count} progress board entries for DateRangeType: {DateRangeType}",
                           progressBoardWithProgressEntryDTos.Count(), request.DateRangeType);

                return ResultT<IEnumerable<ProgressBoardWithProgressEntryDTos>>.Success(progressBoardWithProgressEntryDTos);
           
            }
                _logger.LogError("Invalid DateRangeType: {DateRangeType}. No matching progress board found.", request.DateRangeType);

                return ResultT<IEnumerable<ProgressBoardWithProgressEntryDTos>>.Failure(
                    Error.Failure("400", "Invalid DateRangeType provided"));
        }

        #region Private Methods
        private Dictionary<DateRangeType, Func<CancellationToken,Task<IEnumerable<Domain.Models.ProgressBoard>>>> GetValue()
        {
            return new Dictionary<DateRangeType, Func<CancellationToken, Task<IEnumerable<Domain.Models.ProgressBoard>>>>
            {
                {DateRangeType.Today, 
                    async cancellationToken =>
                        await _progressBoardRepository.GetBoardsByDateRangeAsync(
                            DateTime.UtcNow.Date,
                            DateTime.UtcNow.AddDays(1).AddTicks(-1),
                            cancellationToken) },

                {DateRangeType.Week,
                    async cancellationToken => 
                        await _progressBoardRepository.GetBoardsByDateRangeAsync(
                            DateTime.UtcNow.Date.AddDays(-7),
                            DateTime.UtcNow.Date.AddTicks(-7),
                            cancellationToken)},

                {DateRangeType.Month,
                    async cancellationToken =>
                        await _progressBoardRepository.GetBoardsByDateRangeAsync(
                            new DateTime(DateTime.UtcNow.Year,DateTime.UtcNow.Month, 1),
                            new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(1).AddTicks(-1),
                            cancellationToken)},

                {DateRangeType.Week,
                    async cancellationToken =>
                        await _progressBoardRepository.GetBoardsByDateRangeAsync(
                            new DateTime(DateTime.UtcNow.Year, 1, 1),
                            new DateTime(DateTime.UtcNow.Year + 1 , 1, 1).AddTicks(-1),
                            cancellationToken)},


            };
        }
        #endregion
    }
}
