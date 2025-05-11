using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;

namespace MetaBond.Application.Feature.ProgressEntry.Query.GetProgressEntriesWithAuthor;

public sealed class GetProgressEntriesWithAuthorsQuery : IQuery<IEnumerable<ProgressEntriesWithUserDTos>>
{
    public Guid ProgressEntryId { get; set; }
}