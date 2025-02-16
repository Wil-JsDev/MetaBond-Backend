using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;
using MetaBond.Domain;

namespace MetaBond.Application.Feature.ProgressBoard.Querys.GetRange
{
    public sealed class GetRangeProgressBoardQuerys : IQuery<IEnumerable<ProgressBoardWithProgressEntryDTos>>
    {
        public DateRangeType DateRangeType {  get; set; }
    }
}
