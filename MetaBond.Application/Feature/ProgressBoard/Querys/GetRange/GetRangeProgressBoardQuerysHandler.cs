using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;
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
                if (!progressBoards.Any())
                {
                    logger.LogError("No progress board entries found for DateRangeType: {DateRangeType}", request.DateRangeType);
                    
                    var getMessage = GetMessage().TryGetValue(request.DateRangeType, out var messageValue);
                    if (getMessage)
                    {
                        logger.LogWarning("No progress board found for the given date range");
                        
                        return ResultT<IEnumerable<ProgressBoardWithProgressEntryDTos>>.Failure(Error.NotFound("404", messageValue!));
                    }
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
                )).ToList();
                
                IEnumerable<ProgressBoardWithProgressEntryDTos> progressBoardWithProgressEntryDTos = progressBoards.Select(x => new ProgressBoardWithProgressEntryDTos
                (
                    ProgressBoardId: x.Id,
                    CommunitiesId: x.CommunitiesId,
                    ProgressEntries: progressEntryList ,
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
        private static Dictionary<DateRangeType, string> GetMessage()
        {
            return new Dictionary<DateRangeType, string>
            {
                { DateRangeType.Today, "No records available for today." },
                { DateRangeType.Week, "No records available for this week." },
                { DateRangeType.Month, "No records available for this month." },
                { DateRangeType.Year, "No records available for this year." },
            };
        }
        
        private Dictionary<DateRangeType, Func<CancellationToken,Task<IEnumerable<Domain.Models.ProgressBoard>>>> GetValue()
        {
            return new Dictionary<DateRangeType, Func<CancellationToken, Task<IEnumerable<Domain.Models.ProgressBoard>>>>
            {
                {DateRangeType.Today, 
                    async cancellationToken =>
                        await progressBoardRepository.GetBoardsByDateRangeAsync(
                            DateTime.SpecifyKind(DateTime.UtcNow.Date, DateTimeKind.Utc),  // Aseguramos que la fecha sea UTC
                            DateTime.SpecifyKind(DateTime.UtcNow.AddDays(1).AddTicks(-1), DateTimeKind.Utc),  // Fin del día de hoy en UTC
                            cancellationToken) },

                {DateRangeType.Week,
                    async cancellationToken => 
                        await progressBoardRepository.GetBoardsByDateRangeAsync(
                            DateTime.SpecifyKind(DateTime.UtcNow.Date.AddDays(-7), DateTimeKind.Utc),  // Inicio de la semana pasada en UTC
                            DateTime.SpecifyKind(DateTime.UtcNow.Date.AddDays(-1), DateTimeKind.Utc),  // Fin de la semana pasada en UTC
                            cancellationToken)},

                {DateRangeType.Month,
                    async cancellationToken =>
                        await progressBoardRepository.GetBoardsByDateRangeAsync(
                            DateTime.SpecifyKind(new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1), DateTimeKind.Utc),  // Inicio del mes en UTC
                            DateTime.SpecifyKind(new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(1).AddTicks(-1), DateTimeKind.Utc),  // Fin del mes en UTC
                            cancellationToken)},

                {DateRangeType.Year,
                    async cancellationToken =>
                        await progressBoardRepository.GetBoardsByDateRangeAsync(
                            DateTime.SpecifyKind(new DateTime(DateTime.UtcNow.Year, 1, 1), DateTimeKind.Utc),  // Inicio del año en UTC
                            DateTime.SpecifyKind(new DateTime(DateTime.UtcNow.Year + 1, 1, 1).AddTicks(-1), DateTimeKind.Utc),  // Fin del año en UTC
                            cancellationToken)},
            };
        }
        #endregion
    }
}
