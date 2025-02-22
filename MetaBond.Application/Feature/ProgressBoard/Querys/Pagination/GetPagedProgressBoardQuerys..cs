using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.ProgressBoard.Querys.Pagination
{
    public sealed class GetPagedProgressBoardQuerys : IQuery<PagedResult<ProgressBoardDTos>>
    {
       public int PageNumber { get; set; }

       public int PageSize { get; set; }
    }
}
