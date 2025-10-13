using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.Pagination;
using MetaBond.Domain;

namespace MetaBond.Application.Feature.Admin.Query.GetPagedUserStatus;

public sealed class GetPagedUserStatusQuery : IQuery<PagedResult<UserDTos>>
{
    public StatusAccount StatusAccount { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}