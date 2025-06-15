using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.User;

namespace MetaBond.Application.Feature.User.Query.SearchByUsername;

public sealed class SearchByUsernameUserQuery : IQuery<IEnumerable<UserDTos>>
{
    public string? Username { get; set; }
}