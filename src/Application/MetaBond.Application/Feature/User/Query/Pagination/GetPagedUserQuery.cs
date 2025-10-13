using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.User.Query.Pagination;

public sealed class GetPagedUserQuery : IQuery<PagedResult<UserDTos>>
{
    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}