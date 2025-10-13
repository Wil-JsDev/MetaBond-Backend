using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.User.Query.SearchByUsername;

public sealed class SearchByUsernameUserQuery : IQuery<PagedResult<UserDTos>>
{
    public string? Username { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}