using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.ProgressBoard.Query.GetProgressBoardsWithAuthor;

public sealed class GetProgressBoardsWithAuthorQuery : IQuery<PagedResult<ProgressBoardWithUserDTos>>
{
    public Guid ProgressBoardId { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}