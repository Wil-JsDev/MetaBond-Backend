using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.ProgressEntry.Query.GetProgressEntriesWithAuthor;

public sealed class GetProgressEntriesWithAuthorsQuery : IQuery<PagedResult<ProgressEntriesWithUserDTos>>
{
    public Guid ProgressEntryId { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}