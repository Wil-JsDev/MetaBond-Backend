using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;

namespace MetaBond.Application.Feature.ProgressBoard.Commands.Create
{
    public sealed class CreateProgressBoardCommand : ICommand<ProgressBoardDTos>
    {
        public Guid CommunitiesId { get; set; }
    }
}
