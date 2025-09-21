using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.ProgressEntry.Query.GetOrderById;

public sealed class GetOrderByIdProgressEntryQuery : IQuery<PagedResult<ProgressEntryBasicDTos>>
{
    public Guid ProgressBoardId { get; set; }
    
    public int PageNumber { get; set; }
    
    public int PageSize { get; set; }
}