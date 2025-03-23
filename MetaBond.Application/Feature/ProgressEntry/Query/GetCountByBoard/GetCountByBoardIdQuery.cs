using MetaBond.Application.Abstractions.Messaging;

namespace MetaBond.Application.Feature.ProgressEntry.Query.GetCountByBoard;

public sealed class GetCountByBoardIdQuery : IQuery<int>
{
    public Guid ProgressBoardId { get; set; }
}