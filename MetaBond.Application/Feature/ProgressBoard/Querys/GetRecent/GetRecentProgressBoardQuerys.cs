using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;
using MetaBond.Domain;

namespace MetaBond.Application.Feature.ProgressBoard.Querys.GetRecent
{
    public sealed class GetRecentProgressBoardQuerys : IQuery<IEnumerable<ProgressBoardDTos>>
    {
        public DateRangeFilter dateFilter {  get; set; }
    }
}
