using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;
using MetaBond.Domain;

namespace MetaBond.Application.Feature.ProgressBoard.Querys.GetRange
{
    public sealed class GetRangeProgressBoardQuerys : IQuery<IEnumerable<ProgressBoardWithProgressEntryDTos>>
    {
        public int Page { get; set; }
        
        public int PageSize { get; set; }
        public DateRangeType DateRangeType {  get; set; }
    }
}
