using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.ProgressEntry.Query.Pagination;

public sealed class GetPagedProgressEntryQuery : IQuery<PagedResult<ProgressEntryDTos>>
{
    public int PageNumber {  get; set; }

    public int PageSize { get; set; }
}