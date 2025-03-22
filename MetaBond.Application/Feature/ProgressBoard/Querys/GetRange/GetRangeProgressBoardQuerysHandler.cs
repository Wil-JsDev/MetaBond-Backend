using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressBoard.Querys.GetRange
{
    internal sealed class GetRangeProgressBoardQuerysHandler(
        IProgressBoardRepository progressBoardRepository,
        IProgressEntryRepository progressEntryRepository,
        ILogger<GetRangeProgressBoardQuerysHandler> logger)
        : IQueryHandler<GetRangeProgressBoardQuerys, IEnumerable<ProgressBoardWithProgressEntryDTos>>
    {
        public async Task<ResultT<IEnumerable<ProgressBoardWithProgressEntryDTos>>> Handle(
            GetRangeProgressBoardQuerys request, 
            CancellationToken cancellationToken)
        {
            var progressBoard = GetValue();
            if (progressBoard.TryGetValue((request.DateRangeType), out var progressBoardValue))
            {
                var progressBoardList = await progressBoardValue(cancellationToken);
                IEnumerable<Domain.Models.ProgressBoard> progressBoards = progressBoardList.ToList();
                if ( !progressBoards.Any())
                {
                    logger.LogError("No progress board entries found for DateRangeType: {DateRangeType}", request.DateRangeType);

                    return ResultT<IEnumerable<ProgressBoardWithProgressEntryDTos>>.Failure(
                        Error.Failure("400", "The list is empty")
                    );
                }

                if (request.Page <= 0 || request.PageSize <= 0)
                {
                    logger.LogWarning("Invalid pagination parameters: Page = {Page}, PageSize = {PageSize}", request.Page, request.PageSize);

                    return ResultT<IEnumerable<ProgressBoardWithProgressEntryDTos>>.Failure(
                        Error.Failure("400", "Page number and page size must be greater than zero. Please provide valid pagination values."));
                }
                
                var progressEntryPaged = await progressEntryRepository.GetPagedProgressEntryAsync(
                    request.PageSize, 
                    request.Page, 
                    cancellationToken);
                
                var progressEntryList = progressEntryPaged.Items!.Select(x => new ProgressEntrySummaryDTos
                (
                    ProgressEntryId: x.Id,
                    Description: x.Description,
                    CreatedAt: x.CreatedAt,
                    ModifiedAt: x.UpdateAt
                ));
                
                IEnumerable<ProgressBoardWithProgressEntryDTos> progressBoardWithProgressEntryDTos = progressBoards.Select(x => new ProgressBoardWithProgressEntryDTos
                (
                    ProgressBoardId: x.Id,
                    CommunitiesId: x.CommunitiesId,
                    ProgressEntries: x.ProgressEntries != null ?
                        progressEntryList.Select(pe => new ProgressEntrySummaryDTos
                        (
                            ProgressEntryId: pe.ProgressEntryId,
                            Description: pe.Description,
                            CreatedAt: pe.CreatedAt,
                            ModifiedAt: pe.ModifiedAt
                        )).ToList() : 
                        new List<ProgressEntrySummaryDTos>(),
                    CreatedAt: x.CreatedAt,
                    UpdatedAt: x.UpdatedAt
                ));

                IEnumerable<ProgressBoardWithProgressEntryDTos> progressBoardWithProgressEntryDTosEnumerable = progressBoardWithProgressEntryDTos.ToList();
                logger.LogInformation("Successfully retrieved {Count} progress board entries for DateRangeType: {DateRangeType}",
                           progressBoardWithProgressEntryDTosEnumerable.Count(), request.DateRangeType);

                return ResultT<IEnumerable<ProgressBoardWithProgressEntryDTos>>.Success(progressBoardWithProgressEntryDTosEnumerable);
           
            }
            logger.LogError("Invalid DateRangeType: {DateRangeType}. No matching progress board found.", request.DateRangeType);

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
                        await progressBoardRepository.GetBoardsByDateRangeAsync(
                            DateTime.UtcNow.Date,
                            DateTime.UtcNow.AddDays(1).AddTicks(-1),
                            cancellationToken) },

                {DateRangeType.Week,
                    async cancellationToken => 
                        await progressBoardRepository.GetBoardsByDateRangeAsync(
                            DateTime.UtcNow.Date.AddDays(-7),
                            DateTime.UtcNow.Date.AddTicks(-7),
                            cancellationToken)},

                {DateRangeType.Month,
                    async cancellationToken =>
                        await progressBoardRepository.GetBoardsByDateRangeAsync(
                            new DateTime(DateTime.UtcNow.Year,DateTime.UtcNow.Month, 1),
                            new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(1).AddTicks(-1),
                            cancellationToken)},

                {DateRangeType.Week,
                    async cancellationToken =>
                        await progressBoardRepository.GetBoardsByDateRangeAsync(
                            new DateTime(DateTime.UtcNow.Year, 1, 1),
                            new DateTime(DateTime.UtcNow.Year + 1 , 1, 1).AddTicks(-1),
                            cancellationToken)},


            };
        }
        #endregion
    }
}
