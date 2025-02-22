using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;

namespace MetaBond.Application.Feature.ProgressBoard.Commands.Delete
{
    public sealed class DeleteProgressBoardCommand : ICommand<Guid>
    {
        public Guid ProgressBoardId  { get; set; }
    }
}
