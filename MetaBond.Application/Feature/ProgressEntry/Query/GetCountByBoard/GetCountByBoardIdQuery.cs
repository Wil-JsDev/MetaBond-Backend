using MetaBond.Application.Abstractions.Messaging;

namespace MetaBond.Application.Feature.ProgressEntry.Querys.GetCountByBoard
{
    public sealed class GetCountByBoardIdQuery : IQuery<int>
    {
        public Guid ProgressBoardId { get; set; }
    }
}
