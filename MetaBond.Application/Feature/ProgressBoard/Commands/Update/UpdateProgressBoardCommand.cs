using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;

namespace MetaBond.Application.Feature.ProgressBoard.Commands.Update;

public sealed class UpdateProgressBoardCommand : ICommand<ProgressBoardDTos>
{
    public Guid ProgressBoardId { get; set; }

    public Guid CommunitiesId { get; set; }

    public Guid UserId { get; set; }
}