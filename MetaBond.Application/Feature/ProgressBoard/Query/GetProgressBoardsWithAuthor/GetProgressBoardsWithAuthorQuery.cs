using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;

namespace MetaBond.Application.Feature.ProgressBoard.Query.GetProgressBoardsWithAuthor;

public sealed class GetProgressBoardsWithAuthorQuery : IQuery<IEnumerable<ProgressBoardWithUserDTos>>
{
    public Guid ProgressBoardId { get; set; }
}