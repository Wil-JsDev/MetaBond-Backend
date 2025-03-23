using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;

namespace MetaBond.Application.Feature.ProgressBoard.Query.GetById;

public sealed class GetByIdProgressBoardQuery : IQuery<ProgressBoardDTos>
{
    public Guid ProgressBoardId { get; set; }
}