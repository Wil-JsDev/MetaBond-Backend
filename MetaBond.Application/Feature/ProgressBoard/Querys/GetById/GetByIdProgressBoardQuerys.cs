using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;

namespace MetaBond.Application.Feature.ProgressBoard.Querys.GetById
{
    public sealed class GetByIdProgressBoardQuerys : IQuery<ProgressBoardDTos>
    {
        public Guid ProgressBoardId { get; set; }
    }
}
