﻿using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.ProgressBoard.Query.Pagination;

public sealed class GetPagedProgressBoardQuery : IQuery<PagedResult<ProgressBoardDTos>>
{
    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}